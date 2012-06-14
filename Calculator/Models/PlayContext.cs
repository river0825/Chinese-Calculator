using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Calculator.Models
{
    public class PlayContext
    {
        string dbFile = "./data.db";

        public PlayContext()
        {
            if (File.Exists(dbFile))
            {
                //load data
                using (FileStream stm =
                        new FileStream(dbFile, FileMode.Open))
                {
                    using (GZipStream gstm = new GZipStream(stm, CompressionMode.Compress))
                    {
                        using (StreamReader sr = new StreamReader(gstm))
                        {
                            //還原後一樣取出第indexToTest筆的User顯示內容
                            PlayRecords = JsonConvert.DeserializeObject<IList<PlayRecord>>(sr.ReadToEnd());
                        }
                    }
                }
            }


            PlayRecords = new List<PlayRecord>();

            
        }

        public void Save(){
            using (FileStream stm =
                    new FileStream(dbFile, FileMode.Create))
            {
                using (GZipStream gstm = new GZipStream(stm, CompressionMode.Compress))
                {
                    using (StreamWriter sr = new StreamWriter(gstm))
                    {
                        sr.Write(JsonConvert.SerializeObject(PlayRecords));
                    }
                }
            }
        }

        public IList<PlayRecord> PlayRecords { get; private set; }
    }
}
