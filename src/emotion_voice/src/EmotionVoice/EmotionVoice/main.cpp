// Main
// author: Johannes Wagner <wagner@hcm-lab.de>
// created: 2008/12/29
// Copyright (C) University of Augsburg, Lab for Human Centered Multimedia
//
// *************************************************************************************************
//
// This file is part of Social Signal Interpretation (SSI) developed at the 
// Lab for Human Centered Multimedia of the University of Augsburg
//
// This library is free software; you can redistribute itand/or
// modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or any laterversion.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FORA PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along withthis library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//*************************************************************************************************

#include "ssi.h"

#include "audio/include/ssiaudio.h"

#include "model/include/ssimodel.h"
#include "ssiml.h"
#include "ssiev.h"

using namespace ssi;
int solve_cmd_arg(int argc, const char* argv[]);
Classifier* loadClassifier();
bool ex_continuous(std::string wav_file, std::string result_file, ssi_time_t wav_time);

ssi_time_t wav_time = 6.0;
ssi_size_t sleep_dur = 1000;
const char* train_file_path = "model/ev_cont";
std::string result_file_path = "result/emotion";
Classifier* classifier = 0;

bool debug = false;
bool use_microphone = false;
bool output_to_console = false;
bool plot_audio = false;
std::string framesize = "1.0s";

int main(int argc, const char* argv[]) {
	// 处理 option
	int begin_i = solve_cmd_arg(argc, argv);
	if (begin_i == -1) return 0;

	ssi_print("%s\n\nbuild version: %s\n\n", SSI_COPYRIGHT, SSI_VERSION);

	Factory::RegisterDLL("ssiframe.dll");
	Factory::RegisterDLL("ssievent.dll");
	Factory::RegisterDLL("ssiemovoice.dll");
	Factory::RegisterDLL("ssigraphic.dll");
	Factory::RegisterDLL("ssimodel.dll");
	Factory::RegisterDLL("ssiaudio.dll");
	Factory::RegisterDLL("ssisignal.dll");
	Factory::RegisterDLL("ssiioput.dll");

	classifier = loadClassifier();

	for (int i = 0; i < (argc - begin_i) / 3; i++){
		ssi_time_t time = std::atof(argv[3 * i + begin_i + 2]);
		if (strcmp(argv[3 * i + begin_i + 2],"0") == 0 || time > 0){
			ex_continuous(argv[3 * i + begin_i], argv[3 * i + begin_i + 1], time);
		}
		else{
			ssi_wrn("the time format is not right for : %s", argv[3 * i + begin_i + 2]);
		}
	}

	Factory::Clear();
	return 0;
}

Classifier* loadClassifier(){
	Trainer ev_class;
	if (!Trainer::Load(ev_class, train_file_path)) {
		ssi_err("could not load model");
	}
	Classifier *classifier = ssi_pcast(Classifier, Factory::Create(Classifier::GetCreateName()));
	classifier->getOptions()->setTrainer(train_file_path);
	// 因为setEventListener 会使得classifier从options中重新加载模型，即使在这里设置了模型，在那里也会被清除
	//classifier->setTrainer(&ev_class);
	if (output_to_console)
		classifier->getOptions()->console = true;
	else
		classifier->getOptions()->console = false;
	return classifier;
}

bool checkEnd(ITheFramework* frame, int buffer_id, ssi_time_t wav_time){
	ssi_time_t currentSampleTime, runningTime;
	runningTime = frame->GetElapsedTime();
	frame->GetCurrentSampleTime(buffer_id, currentSampleTime);
	//ssi_print("time : %lf, %lf\n",runningTime,currentSampleTime);
	return currentSampleTime > wav_time || runningTime > wav_time;
}

bool ex_continuous(std::string wav_file, std::string result_file, ssi_time_t wav_time) {
	IThePainter *painter = Factory::GetPainter();
	ITheEventBoard *board = Factory::GetEventBoard();
	ITheFramework *frame = Factory::GetFramework();
	//ssi_pcast(TheFramework, frame)->getOptions()->setConsolePos(400, 0, 400, 400);
	// 倒计时
	ssi_size_t cuntdown = 0;
	frame->getOptions()->setOptionValue("countdown", &cuntdown);

	ISensor* audio;
	ITransformable *audio_p;
	//sensor audio	扬声器	
	if (use_microphone || wav_file == "0"){
		Audio* _audio = ssi_pcast(Audio, Factory::Create(Audio::GetCreateName(), "audio"));
		_audio->getOptions()->scale = true;
		_audio->getOptions()->sr = 16000.0;
		audio_p = frame->AddProvider(_audio, SSI_AUDIO_PROVIDER_NAME);
		frame->AddSensor(_audio);
		audio = _audio;
	}
	// 读取wav文件
	else{
		WavReader* _audio = ssi_pcast(WavReader, Factory::Create(WavReader::GetCreateName(), "reader"));
		_audio->getOptions()->loop = false;
		_audio->getOptions()->scale = true;
		_audio->getOptions()->setOptionValue("path", (char*)(wav_file.c_str()));
		audio_p = frame->AddProvider(_audio, SSI_AUDIO_PROVIDER_NAME);
		frame->AddSensor(_audio);
		audio = _audio;
	}

	// classifier
	frame->AddConsumer(audio_p, classifier, framesize.c_str());

	// object 
	if (result_file != "0"){
		FileEventWriter* eventwriter = ssi_pcast(FileEventWriter, Factory::Create(FileEventWriter::GetCreateName()));
		eventwriter->getOptions()->setOptionValue("path", (char*)(result_file.c_str()));
		board->RegisterSender(*classifier);
		board->RegisterListener(*eventwriter, classifier->getEventAddress());
	}

	// plot	
	if (plot_audio){
		SignalPainter *sigpaint = 0;
		sigpaint = ssi_pcast(SignalPainter, Factory::Create(SignalPainter::GetCreateName()));
		sigpaint->getOptions()->size = 10.0;
		sigpaint->getOptions()->setName("audio");
		sigpaint->getOptions()->type = PaintSignalType::AUDIO;
		frame->AddConsumer(audio_p, sigpaint, "0.2s");
	}

	// run framework
	frame->Start();
	board->Start();
	painter->Arrange(1, 1, 0, 0, 400, 400); // audio painter

	if (wav_time == 0)// 不限制时间
	{ 
		frame->Wait();
	}
	else{
		while (!checkEnd(frame, audio_p->getBufferId(), wav_time)){
			ssi_sleep(sleep_dur);
		}
	}

	board->Stop();
	frame->Stop();
	frame->Clear();
	board->Clear();
	painter->Clear();
	return true;
};

int solve_cmd_arg(int argc, const char* argv[]){

#define PRINT_HELP ssi_print("usage : EmotionVoice [-microphone] [-console] [-plot] [-famesize time] (wavfile\n");\
                   ssi_print("                     resultfile time)*\n\n"); \
				   ssi_print("  -microphone / -m : recognize voice from microphone not wavfile \n");\
				   ssi_print("                     (识别从麦克风中获取的声音)\n"); \
				   ssi_print("  -console / -c    : print result on console\n");\
				   ssi_print("                     (将识别结果显示到控制台中)\n"); \
				   ssi_print("  -plot / -p       : show voice activity (显示声音的强度)\n");\
				   ssi_print("  -framesize / -f  : get a result per time second, default is 1.0s\n");\
				   ssi_print("                     (每 time 秒时间的声音获得一个结果, 默认是 1 秒)\n"); \
				   ssi_print("  wavfile.wav      : wav file use to recognize,set 0 when use microphone. such \n");\
				   ssi_print("                     as test.wav.\n");\
				   ssi_print("                     (用于识别的wav文件名,当使用麦克风时时设置为0。例如test.wav)\n");\
				   ssi_print("  resultfile       : the name of file to save recognize result, set 0 when do \n");\
                   ssi_print("                     not use it.such as result\n");\
				   ssi_print("                     (用于存储识别结果的文件名，当不许存储时设置为0，例如result)\n");\
				   ssi_print("  time             : the time of the wav in second.(wav文件的时间长度，单位是秒)\n\n");\
				   ssi_wrn("warnning : the last three argument can not ignore, even when it is useless.use 0 0 0. (最后的三个参数不能不写，当不需要它们时，设置为 0 0 0)"); \
				   ssi_print("\nexample  : EmotionVoice test/test.wav result 10\n");\
				   ssi_print("           EmotionVoice test1.wav result 10 test2.wav 12\n");\
				   ssi_print("           EmotionVoice -m 0 0 0\n");\
				   ssi_print("           EmotionVoice -c -p test/test.wav result 10\n");\
				   ssi_print("           EmotionVoice -f 1.2 test/test.wav result 10\n")

	int begin_i = 1;
	if (argc < 2) {
		PRINT_HELP;
		return -1;
	}
	while (argv[begin_i] && argv[begin_i][0] == '-'){
		if (strcmp(argv[begin_i], "-microphone") == 0 || strcmp(argv[begin_i],"-m") == 0){
			use_microphone = true;
		}
		else if (strcmp(argv[begin_i], "-console") == 0 || strcmp(argv[begin_i], "-c") == 0)
			output_to_console = true;
		else if (strcmp(argv[begin_i], "-plot") == 0 || strcmp(argv[begin_i], "-p") == 0)
			plot_audio = true;
		else if (strcmp(argv[begin_i], "-framesize") == 0 || strcmp(argv[begin_i], "-f") == 0){
			begin_i++;
			if (begin_i >= argc || std::atof(argv[begin_i]) <= 0) {
				PRINT_HELP;
				return -1;
			}
			framesize = argv[begin_i]; framesize = framesize +"s";
		}
		begin_i++;
		if (begin_i >= argc) {
			PRINT_HELP;
			return -1;
		}
	}

#undef PRINT_HELP
	return begin_i;
}
