1.	���bat�ļ�����

2.  ��������
	����˷���ʵʱʶ�� ��
		EmotionVoice -m -c -p 0 result 0
	ʶ���ļ�������תΪ������16000����������wav�ļ���,���һ�������������ĳ��ȣ���λΪ��
		EmotionVoice -c -p test/test.wav result 10
		
3.	�ڿ���̨������棨ע��һ�������������Ĵ��ڣ����س���ǿ�ƽ�������
		
4. ��ϸ�Ĳ������Բμ������б�
usage : EmotionVoice [-microphone] [-console] [-plot] [-famesize time] (wavfile resultfile time)*

  -microphone / -m : recognize voice from microphone not wavfile
                     (ʶ�����˷��л�ȡ������)
  -console / -c    : print result on console
                     (��ʶ������ʾ������̨��)
  -plot / -p       : show voice activity (��ʾ������ǿ��)
  -framesize / -f  : get a result per time second, default is 1.0s
                     (ÿ time ��ʱ�������ʶ����һ�����, Ĭ���� 1 ��)
  wavfile.wav      : wav file use to recognize,set 0 when use microphone. such
                     as test.wav.
                     (����ʶ���wav�ļ���,��ʹ����˷�ʱʱ����Ϊ0������test.wav)

  resultfile       : the name of file to save recognize result, set 0 when do
                     not use it.such as result
                     (���ڴ洢ʶ�������ļ�����������洢ʱ����Ϊ0������result)

  time             : the time of the wav in second.(wav�ļ���ʱ�䳤�ȣ���λ����)


[__________] # WARNING # warnning : the last three argument can not ignore, even
 when it is useless.use 0 0 0. (���������������ܲ�д��������Ҫ����ʱ������Ϊ 0
 0 0)
location: main.cpp (210)

example  : EmotionVoice test/test.wav result 10
           EmotionVoice test1.wav result 10 test2.wav 12
           EmotionVoice -m 0 0 0
           EmotionVoice -c -p test/test.wav result 10
           EmotionVoice -f 1.2 test/test.wav result 10
