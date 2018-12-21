using System;
using System.Collections.Generic;
using System.IO;

namespace MatrixToBeds
{
    class Program
    {
        static void Main(string[] args)
        {
            int minReadCount = 1;
            string rPath = @"/Users/vahid/atac_seq_data/";
            string path = @"/Users/vahid/atac_seq_data/04_Counts.matrix";
            var fileReader = new StreamReader(path);
            var headerLine = fileReader.ReadLine();
            var headers = headerLine.Split('\t');
            var writters = GetWritters(rPath, headers);

            int i = 0;
            string line = "";
            string peak;
            string[] splitLine;
            while ((line = fileReader.ReadLine()) != null)
            {
                splitLine = line.Split('\t');
                peak = string.Join('\t', splitLine[0].Split('_'));
                for (i = 1; i < splitLine.Length; i++)
                    if (int.Parse(splitLine[i]) >= minReadCount)
                        writters[i - 1].WriteLine(peak);
            }

            // Close all file handlers
            fileReader.Close();
            fileReader.Dispose();
            foreach (var writter in writters)
            {
                writter.Close();
                writter.Dispose();
            }
        }

        static List<StreamWriter> GetWritters(string path, string[] columns)
        {
            var rtv = new List<StreamWriter>();
            foreach (var barcode in columns)
                rtv.Add(new StreamWriter(path + barcode + ".bed"));
            return rtv;
        }
    }
}
