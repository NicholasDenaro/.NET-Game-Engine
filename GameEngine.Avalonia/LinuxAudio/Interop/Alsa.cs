using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI.LinuxAudio.Interop
{
    static class Alsa
    {
        const string library = "libasound";

        [DllImport(library)]
        public static extern IntPtr snd_strerror(int errnum);

        [DllImport(library)]
        public static extern int snd_pcm_open(ref IntPtr pcm, string name, snd_pcm_stream_t stream, int mode); // name of the device, "default"

        [DllImport(library)]
        public static extern int snd_pcm_start(IntPtr pcm); // start

        [DllImport(library)]
        public static extern int snd_pcm_reset(IntPtr pcm); // reset back to beginning of buffer

        [DllImport(library)]
        public static extern int snd_pcm_pause(IntPtr pcm, int enable); // toggles pause, 0 to play, 1 to pause

        [DllImport(library)]
        public static extern int snd_pcm_resume(IntPtr pcm); // resume

        [DllImport(library)]
        public static extern int snd_pcm_drain(IntPtr pcm); // stop, but continues playing to end of buffer

        [DllImport(library)]
        public static extern int snd_pcm_drop(IntPtr pcm); // immediately stop

        [DllImport(library)]
        public static extern int snd_pcm_close(IntPtr pcm); // closes

        [DllImport(library)]
        public static extern int snd_pcm_recover(IntPtr pcm, int err, int silent);

        [DllImport(library)]
        public static extern int snd_pcm_writei(IntPtr pcm, IntPtr buffer, ulong size); // Write data to the audio buffer

        [DllImport(library)]
        public static extern int snd_pcm_writen(IntPtr pcm, ref IntPtr buffers, ulong size); // buffers is an array of buffers

        [DllImport(library)]
        public static extern int snd_pcm_readi(IntPtr pcm, IntPtr buffer, ulong size);

        [DllImport(library)]
        public static extern int snd_pcm_htimestamp(IntPtr pcm, IntPtr avail, IntPtr tstamp); // tstamp is a timespec


        [DllImport(library)]
        public static extern int snd_pcm_hw_params_malloc(ref IntPtr ptr);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_any(IntPtr ptr, IntPtr prms); // array snd_pcm_hw_params_t

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_set_access(IntPtr ptr, IntPtr prms, snd_pcm_access_t access);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_set_format(IntPtr ptr, IntPtr prms, snd_pcm_format_t format);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_set_rate_near(IntPtr ptr, IntPtr prms, IntPtr rate, IntPtr direction);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_set_channels(IntPtr ptr, IntPtr prms, uint channels);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_set_periods(IntPtr ptr, IntPtr prms, uint val, int dir);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_get_period_time(IntPtr prms, IntPtr val, IntPtr dir);

        [DllImport(library)]
        public static extern int snd_pcm_hw_params_set_buffer_size_near(IntPtr ptr, IntPtr prms, IntPtr uframes); //uframes ulong

        [DllImport(library)]
        public static extern int snd_pcm_hw_params(IntPtr ptr, IntPtr prms);

        [DllImport(library)]
        public static extern int snd_pcm_sw_params_malloc(ref IntPtr ptr);

        [DllImport(library)]
        public static extern int snd_pcm_sw_params_current(IntPtr ptr, IntPtr prms);

        [DllImport(library)]
        public static extern int snd_pcm_sw_params_set_avail_min(IntPtr ptr, IntPtr prms, ulong uframes);

        [DllImport(library)]
        public static extern int snd_pcm_sw_params_set_start_threshold(IntPtr ptr, IntPtr prms, ulong uframes);

        [DllImport(library)]
        public static extern int snd_pcm_sw_params(IntPtr ptr, IntPtr prms);


    }
    struct timespec
    {
        public int tv_sec;
        public long tv_nsec;
    };

    enum snd_pcm_stream_t
    {
        SND_PCM_STREAM_PLAYBACK = 0,
        SND_PCM_STREAM_CAPTURE = 1,
        SND_PCM_STREAM_LAST = SND_PCM_STREAM_CAPTURE,
    }

    enum snd_pcm_format_t
    {
        SND_PCM_FORMAT_UNKNOWN = -1,
        SND_PCM_FORMAT_S8 = 0,
        SND_PCM_FORMAT_U8 = 1,
        SND_PCM_FORMAT_S16_LE = 2,
        SND_PCM_FORMAT_S16_BE = 3,
        SND_PCM_FORMAT_U16_LE = 4,
        SND_PCM_FORMAT_U16_BE = 5,
        SND_PCM_FORMAT_S24_LE = 6,
        SND_PCM_FORMAT_S24_BE = 7,
        SND_PCM_FORMAT_U24_LE = 8,
        SND_PCM_FORMAT_U24_BE = 9,
        SND_PCM_FORMAT_S32_LE = 10,
        SND_PCM_FORMAT_S32_BE = 11,
        SND_PCM_FORMAT_U32_LE = 12,
        SND_PCM_FORMAT_U32_BE = 13,
        SND_PCM_FORMAT_FLOAT_LE = 14,
        SND_PCM_FORMAT_FLOAT_BE = 15,
        SND_PCM_FORMAT_FLOAT64_LE = 16,
        SND_PCM_FORMAT_FLOAT64_BE = 17,
        SND_PCM_FORMAT_IEC958_SUBFRAME_LE = 18,
        SND_PCM_FORMAT_IEC958_SUBFRAME_BE = 19,
        SND_PCM_FORMAT_MU_LAW = 20,
        SND_PCM_FORMAT_A_LAW = 21,
        SND_PCM_FORMAT_IMA_ADPCM = 22,
        SND_PCM_FORMAT_MPEG = 23,
        SND_PCM_FORMAT_GSM = 24,
        SND_PCM_FORMAT_SPECIAL = 31,
        SND_PCM_FORMAT_S24_3LE = 32,
        SND_PCM_FORMAT_S24_3BE = 33,
        SND_PCM_FORMAT_U24_3LE = 34,
        SND_PCM_FORMAT_U24_3BE = 35,
        SND_PCM_FORMAT_S20_3LE = 36,
        SND_PCM_FORMAT_S20_3BE = 37,
        SND_PCM_FORMAT_U20_3LE = 38,
        SND_PCM_FORMAT_U20_3BE = 39,
        SND_PCM_FORMAT_S18_3LE = 40,
        SND_PCM_FORMAT_S18_3BE = 41,
        SND_PCM_FORMAT_U18_3LE = 42,
        SND_PCM_FORMAT_U18_3BE = 43,
        SND_PCM_FORMAT_G723_24 = 44,
        SND_PCM_FORMAT_G723_24_1B = 45,
        SND_PCM_FORMAT_G723_40 = 46,
        SND_PCM_FORMAT_G723_40_1B = 47,
        SND_PCM_FORMAT_DSD_U8 = 48,
        SND_PCM_FORMAT_DSD_U16_LE = 49,
        SND_PCM_FORMAT_DSD_U32_LE = 50,
        SND_PCM_FORMAT_DSD_U16_BE = 51,
        SND_PCM_FORMAT_DSD_U32_BE = 52,
        SND_PCM_FORMAT_LAST = SND_PCM_FORMAT_DSD_U32_BE,
    }

    enum snd_pcm_access_t
    {
        SND_PCM_ACCESS_MMAP_INTERLEAVED = 0,
        SND_PCM_ACCESS_MMAP_NONINTERLEAVED = 1,
        SND_PCM_ACCESS_MMAP_COMPLEX = 2,
        SND_PCM_ACCESS_RW_INTERLEAVED = 3,
        SND_PCM_ACCESS_RW_NONINTERLEAVED = 4,
        SND_PCM_ACCESS_LAST = SND_PCM_ACCESS_RW_NONINTERLEAVED,
    }

    enum snd_mixer_selem_channel_id
    {
        SND_MIXER_SCHN_UNKNOWN = -1,
        SND_MIXER_SCHN_FRONT_LEFT = 0,
        SND_MIXER_SCHN_FRONT_RIGHT = 1,
        SND_MIXER_SCHN_REAR_LEFT = 2,
        SND_MIXER_SCHN_REAR_RIGHT = 3,
        SND_MIXER_SCHN_FRONT_CENTER = 4,
        SND_MIXER_SCHN_WOOFER = 5,
        SND_MIXER_SCHN_SIDE_LEFT = 6,
        SND_MIXER_SCHN_SIDE_RIGHT = 7,
        SND_MIXER_SCHN_REAR_CENTER = 8,
        SND_MIXER_SCHN_LAST = 31,
        SND_MIXER_SCHN_MONO = SND_MIXER_SCHN_FRONT_LEFT
    }
}
