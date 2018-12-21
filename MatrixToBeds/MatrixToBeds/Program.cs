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

            OrganizeFilesInFoldersBasedOnCluster(rPath + "/cells", @"/Users/vahid/atac_seq_data/01_Cluster_Identity.annot");
        }

        static List<StreamWriter> GetWritters(string path, string[] columns)
        {
            var rtv = new List<StreamWriter>();
            if (!Directory.Exists(path + "cells /"))
                Directory.CreateDirectory(path + "cells/");
            foreach (var barcode in columns)
                rtv.Add(new StreamWriter(path + "cells/" + barcode + ".bed"));
            return rtv;
        }

        static void OrganizeFilesInFoldersBasedOnCluster(string path, string clusterAnnotation)
        {
            using (StreamReader reader = new StreamReader(clusterAnnotation))
            {
                string annotation = "";
                string[] splitAnnotation;
                while((annotation=reader.ReadLine()) != null)
                {
                    splitAnnotation = annotation.Split('\t');
                    if (!Directory.Exists(path + "/" + splitAnnotation[1]))
                        Directory.CreateDirectory(path + "/" + splitAnnotation[1]);
                    File.Move(path + "/" + splitAnnotation[0] + ".bed", path + "/" + splitAnnotation[1] + "/" + splitAnnotation[0] + ".bed");
                }
            }

            Directory.CreateDirectory(path + "/NotClustered");
            foreach (var file in Directory.GetFiles(path))
                File.Move(file, path + "/NotClustered/" + Path.GetFileName(file));
        }
    }
}
