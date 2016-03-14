1.	点击bat文件运行

2.  输入命令
	从麦克风中实时识别 ：
		EmotionVoice -m -c -p 0 result 0
	识别文件（必须转为采样率16000，单声道的wav文件）,最后一个参数是声音的长度，单位为秒
		EmotionVoice -c -p test/test.wav result 10
		
3.	在控制台程序界面（注意一定不能是其他的窗口），回车将强制结束程序
		
4. 详细的参数可以参见下面列表
usage : EmotionVoice [-microphone] [-console] [-plot] [-famesize time] (wavfile resultfile time)*

  -microphone / -m : recognize voice from microphone not wavfile
                     (识别从麦克风中获取的声音)
  -console / -c    : print result on console
                     (将识别结果显示到控制台中)
  -plot / -p       : show voice activity (显示声音的强度)
  -framesize / -f  : get a result per time second, default is 1.0s
                     (每 time 秒时间的声音识别获得一个结果, 默认是 1 秒)
  wavfile.wav      : wav file use to recognize,set 0 when use microphone. such
                     as test.wav.
                     (用于识别的wav文件名,当使用麦克风时时设置为0。例如test.wav)

  resultfile       : the name of file to save recognize result, set 0 when do
                     not use it.such as result
                     (用于存储识别结果的文件名，当不许存储时设置为0，例如result)

  time             : the time of the wav in second.(wav文件的时间长度，单位是秒)


[__________] # WARNING # warnning : the last three argument can not ignore, even
 when it is useless.use 0 0 0. (最后的三个参数不能不写，当不需要它们时，设置为 0
 0 0)
location: main.cpp (210)

example  : EmotionVoice test/test.wav result 10
           EmotionVoice test1.wav result 10 test2.wav 12
           EmotionVoice -m 0 0 0
           EmotionVoice -c -p test/test.wav result 10
           EmotionVoice -f 1.2 test/test.wav result 10
