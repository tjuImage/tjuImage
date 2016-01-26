//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK Github:
// https://github.com/Microsoft/ProjectOxfordSDK-Windows
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
package com.microsoft.speech.tts;

import android.media.AudioFormat;
import android.media.AudioManager;
import android.media.AudioTrack;
import android.os.AsyncTask;
import android.util.Log;

import com.microsoft.bing.dss.baselib.silk.SilkWrapper;

public class Synthesizer {

    private Voice m_serviceVoice;
    private Voice m_localVoice;

    public String m_audioOutputFormat = AudioOutputFormat.Raw16Khz16BitMonoPcm;

    private void playSound(final byte[] sound, final Runnable callback)
    {
        if(sound == null || sound.length == 0){
            return;
        }

        AsyncTask.execute(new Runnable() {
            @Override
            public void run() {
                final int SAMPLE_RATE = 16000;
                AudioTrack audioTrack = new AudioTrack(AudioManager.STREAM_MUSIC, SAMPLE_RATE, AudioFormat.CHANNEL_CONFIGURATION_MONO,
                        AudioFormat.ENCODING_PCM_16BIT, AudioTrack.getMinBufferSize(SAMPLE_RATE, AudioFormat.CHANNEL_CONFIGURATION_MONO, AudioFormat.ENCODING_PCM_16BIT), AudioTrack.MODE_STREAM);
                if (audioTrack.getState() == AudioTrack.STATE_INITIALIZED) {
                    audioTrack.play();
                    audioTrack.write(sound, 0, sound.length);
                    audioTrack.stop();
                    audioTrack.release();
                } else {
                }

                if (callback != null) {
                    callback.run();
                }
            }
        });
    }

    public enum ServiceStrategy
    {
        AlwaysService//, WiFiOnly, WiFi3G4GOnly, NoService
    }

    public Synthesizer(String serviceClientId, String serviceClientSecret)
    {
        m_serviceVoice = new Voice("en-US");
        m_localVoice = null;
        m_eServiceStrategy = ServiceStrategy.AlwaysService;
        m_ttsServiceClient = new TtsServiceClient(serviceClientId, serviceClientSecret);
    }

    public void SetVoice(Voice serviceVoice, Voice localVoice)
    {
        m_serviceVoice = serviceVoice;
        m_localVoice = localVoice;
    }

    public void SetServiceStrategy(ServiceStrategy eServiceStrategy){
        m_eServiceStrategy = eServiceStrategy;
    }

    public byte[] Speak(String text)
    {
        String ssml = "<speak version='1.0' xml:lang='" + m_serviceVoice.lang + "'><voice xml:gender='" + m_serviceVoice.gender + "'";
        if (m_eServiceStrategy == ServiceStrategy.AlwaysService) {
            if (m_serviceVoice.voiceName.length() > 0){
                ssml += " name='" + m_serviceVoice.voiceName + "'>";
            }
            else
            {
                ssml += ">";
            }
            ssml +=  text + "</voice></speak>";
        }
        return SpeakSSML(ssml);
    }

    public void SpeakToAudio(String text)
    {
        playSound(Speak(text), null);
    }

    public void SpeakSSMLToAudio(String ssml)
    {
        playSound(SpeakSSML(ssml), null);
    }

    public byte[] SpeakSSML(String ssml)
    {
        byte[] result = null;
        // check current network environment
        // to do...
        //
        if (m_eServiceStrategy == ServiceStrategy.AlwaysService){
            byte[] ret = m_ttsServiceClient.SpeakSSML(ssml);
            if(ret == null || ret.length == 0){
                return null;
            }

            String audioSegmentSSML = new String(ret);
            int nStart = audioSegmentSSML.indexOf("<mstts:audiosegment data=\"");
            int nEnd = audioSegmentSSML.indexOf("\">", nStart);
            audioSegmentSSML = audioSegmentSSML.substring(nStart + "<mstts:audiosegment data=\"".length(), nEnd);
            result = m_silkDecoder.decode(audioSegmentSSML);
        }
        return result;
    }

    private TtsServiceClient m_ttsServiceClient;
    private SilkWrapper m_silkDecoder = new SilkWrapper();
    private ServiceStrategy m_eServiceStrategy;
}
