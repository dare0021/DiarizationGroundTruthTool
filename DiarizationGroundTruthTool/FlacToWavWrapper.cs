using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// http://stoyanov.in/2010/07/26/decoding-flac-audio-files-in-c/
/// </summary>
namespace DiarizationGroundTruthTool
{
    static class FlacToWavWrapper
    {
        public static void run(string inputFile, string outputFile)
        {
            Debug.Assert(Path.GetExtension(inputFile).ToLowerInvariant() == ".flac");

            if (!File.Exists(inputFile))
                throw new ApplicationException("Input file " + inputFile + " cannot be found!");

            using (WavWriter wav = new WavWriter(outputFile))
                using (FlacReader flac = new FlacReader(inputFile, wav))
                    flac.Process();
        }
    }
}
