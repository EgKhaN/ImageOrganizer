using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Models
{
    public class InputModel
    {
        public int Count { get; set; } //= 0;
        public int Parallelism { get; set; } //= 2;
        public string SavePath { get; set; } //= @$"./outputs";
        public InputModel() : this(null,null,null)
        {
        }
        public InputModel(int? count,int? parallelCount, string? savePath)
        {
            this.Count = count ?? 0;
            this.Parallelism = parallelCount ?? 2;
            string defaultOutputPath = @$"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Output";
            this.SavePath = savePath ?? defaultOutputPath;
        }
    }
}
