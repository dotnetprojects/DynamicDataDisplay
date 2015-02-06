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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;


namespace AllergologySample
{
    public struct PointInTime {
        private double verticalValue;
        private DateTime horizontalValue;

        public double VerticalValue {
            get {
                return verticalValue;
            }
            set {
                verticalValue=value;
            }
        }

        public DateTime HorizontalValue{
            get {
                return horizontalValue;
            }
            set {
                horizontalValue = value;
            }
        }

        public PointInTime(double vertical,DateTime horizontal){
            verticalValue=vertical;
            horizontalValue=horizontal;
        }
    }

    
    public class DataProvider
    {
        //public bool FetchByClass = true;
        private Dictionary<string, Dictionary<DateTime, double>> data = null;
        private string filename;

        public DataProvider(string filename) {
            this.filename = filename;
            }

        public DateTime[] GetXcomponents(string alergen) {
                if (data != null)
                {
                    List<DateTime> toReturn = new List<DateTime>();
                    foreach (DateTime iter in data[alergen].Keys) {
                        toReturn.Add(iter);
                    }
                    return toReturn.ToArray();
                }
                else return null;
            }

        public double[] GetYcomponents(string alergen) {
            if (data != null)
            {
                List<double> toReturn = new List<double>();
                foreach (DateTime iter in data[alergen].Keys)
                {
                    toReturn.Add(data[alergen][iter]);
                }
                return toReturn.ToArray();
            }
            else
                return null;
        }

        public string[] GetAlergens() {
            List<string> alergens = new List<string>();
            foreach (string alergen in data.Keys) alergens.Add(alergen);
            return alergens.ToArray();
        }

        public void Load(bool group)
        {
            var assembly = Assembly.GetExecutingAssembly();
            ;

            using (Stream resourceStream = assembly.GetManifestResourceStream("AllergologySample." + filename))
            {
                using (XmlReader reader = XmlReader.Create(resourceStream))
                {

                    StringBuilder alergenBuilder = null;

                    data = new Dictionary<string, Dictionary<DateTime, double>>();
                    int columnsCounter = 0;
                    DateTime currProccessingDate = DateTime.Today;
                    string alergenString = "";
                    double alergenConcentration = 0;
                    bool inTd = false;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.EndElement:
                                if (inTd)
                                {
                                    inTd = false;
                                    columnsCounter++;
                                }
                                else if (reader.Name != "location") //tr closing... Commiting fetched data
                                {
                                    if (!data.ContainsKey(alergenString)) data.Add(alergenString, new Dictionary<DateTime, double>());
                                    if (group)
                                    {
                                        if (!data[alergenString].ContainsKey(currProccessingDate)) data[alergenString].Add(currProccessingDate, alergenConcentration);
                                        else data[alergenString][currProccessingDate] += alergenConcentration;
                                    }
                                    else
                                        data[alergenString][currProccessingDate] = alergenConcentration;
                                }
                                break;
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "location":
                                        break;
                                    case "tr":
                                        columnsCounter = 0;
                                        break;
                                    case "td":
                                        inTd = true;
                                        break;
                                }
                                break;
                            case XmlNodeType.Text:
                                if (inTd)
                                {
                                    switch (columnsCounter)
                                    {
                                        case 0: //date
                                            currProccessingDate = DateTime.Parse(reader.Value);
                                            break;
                                        case 1: //класс алегена
                                            if (group)
                                            {
                                                alergenBuilder = new StringBuilder(reader.Value);
                                                int i = 0;
                                                while (i < alergenBuilder.Length) //triming
                                                {
                                                    if (Char.IsLetterOrDigit(alergenBuilder[i++])) break;
                                                }
                                                alergenBuilder.Remove(0, i - 1);

                                                i = alergenBuilder.Length - 1; // tail trimming
                                                while (i >= 0)
                                                {
                                                    if (Char.IsLetterOrDigit(alergenBuilder[i--])) break;
                                                }
                                                alergenBuilder.Remove(i + 2, alergenBuilder.Length - i - 2);
                                                alergenString = alergenBuilder.ToString();
                                            }
                                            break;

                                        case 2: //название алергена
                                            if (!group)
                                            {
                                                alergenBuilder = new StringBuilder(reader.Value);
                                                int i = 0;
                                                while (i < alergenBuilder.Length) //triming
                                                {
                                                    if (Char.IsLetterOrDigit(alergenBuilder[i++])) break;
                                                }
                                                alergenBuilder.Remove(0, i - 1);

                                                i = alergenBuilder.Length - 1; // tail trimming
                                                while (i >= 0)
                                                {
                                                    if (Char.IsLetterOrDigit(alergenBuilder[i--])) break;
                                                }
                                                alergenBuilder.Remove(i + 2, alergenBuilder.Length - i - 2);
                                                alergenString = alergenBuilder.ToString();
                                            }
                                            break;
                                        case 3: //contration
                                            alergenConcentration = double.Parse(reader.Value);
                                            break;
                                        default: break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
        }
    }
}
