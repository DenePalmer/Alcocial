using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Alcocial
{
    public class Database
    {
        public string name { get; set; }
        public string tag1 { get; set; }
        public string tag2 { get; set; }
        public string tag3 { get; set; }
        public int score { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Database()
        {
            score = 0;
        }
    }
}
