using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
//Adds the service reference, allowing access to the cloud database held in Azure
using Alcocial.ServiceReference1;
using System.Diagnostics;
using System.Device.Location;
//Allows the use of mapping controls such as pushpins etc
using Microsoft.Phone.Controls.Maps;


namespace Alcocial.Pages
{
    public partial class SuggestRoute : PhoneApplicationPage
    {
        //Contains every result from db
        List<Database> pubs = new List<Database>();
        List<Database> results = new List<Database>();
        GeoCoordinateWatcher watcher;


        public SuggestRoute()
        {

            InitializeComponent();

            Service1Client service = new Service1Client();
            service.ViewProfileCompleted += new EventHandler<ViewProfileCompletedEventArgs>(svc_ViewProfileCompleted);
            service.ViewProfileAsync();                       

            //Sets the map zoom bar to visible, and sets the initial zoom level 
            routeMap.ZoomBarVisibility = System.Windows.Visibility.Visible;
            routeMap.ZoomLevel = 14;

            
            if (watcher == null)
            {
                //Gets the highest map accuracy
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                {
                    //Minimum distance to travel before the map location updates
                    MovementThreshold = 10
                };

                routeMap.Center = watcher.Position.Location               

                watcher.StatusChanged += new
                EventHandler<GeoPositionStatusChangedEventArgs>
                (watcher_StatusChanged);
                watcher.Start();
            }

        }


        //Displays feedback to the user if data or mapping is turned off/not available
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs pubs)
        {
            switch (pubs.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("disabled");
                    break;
                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    break;
                case GeoPositionStatus.NoData:
                    Debug.WriteLine("nodata");
                    break;
                case GeoPositionStatus.Ready:
                    Debug.WriteLine("ready");
                    break;
            }
        }


        void svc_ViewProfileCompleted(object sender, ViewProfileCompletedEventArgs f)
        {
            foreach (var a in f.Result)
            {
                pubs.Add(new Database() { name = a.Name, latitude = Convert.ToDouble(a.Latitude), longitude = Convert.ToDouble(a.Longitude), tag1 = a.Tag1, tag2 = a.Tag2, tag3 = a.Tag3 });
                
            }
           
        }

        //Totals the score of each pub in the database according to their tags compared to the user's request
        public int getScore(Database item, List<String> list)
        {
            int score = 0;
            foreach (string tag in list)
            {
                if (tag == item.tag1)
                    score += 3;
                else if (tag == item.tag2)
                    score += 2;
                else if (tag == item.tag3)
                    score++;
            }
            return score;
        }

        //Code is executed upon pressing of the route button
        public void routeButton_Click(object sender, RoutedEventArgs f)
        {
            if (routeMap.Children.Count > 0)
            {
                routeMap.Children.Clear();
            }

            //Plots user push pin to the center of the map 
            Pushpin user = new Pushpin();
            user.Location = routeMap.Center;
                //Sets colour of user push pin to red 
            user.Background = new SolidColorBrush(Colors.Red);
            user.Content = "You are here";
            routeMap.Children.Add(user);

            //Takes the ticked check boxes and forms them into a string List, allowing them to be compared to the database to pull out appropriate pubs
            List<string> drinks = new List<string>();
            if (aleCheck.IsChecked == true)
                drinks.Add(aleCheck.Content.ToString());
            if (lagerCheck.IsChecked == true)
                drinks.Add(lagerCheck.Content.ToString());
            if (ciderCheck.IsChecked == true)
                drinks.Add(ciderCheck.Content.ToString());
            if (wineCheck.IsChecked == true)
                drinks.Add(wineCheck.Content.ToString());
            if (cocktailCheck.IsChecked == true)
                drinks.Add(cocktailCheck.Content.ToString());
            if (shotsCheck.IsChecked == true)
                drinks.Add(shotsCheck.Content.ToString());

            //Contains all locations of their desired choices/tickboxes
            results = new List<Database>();
            foreach (Database outputPubs in pubs)
            {
                outputPubs.score = getScore(outputPubs, drinks);
            }

            //Converts the users required number of pubs into a useable integer
            int numofpubs = Convert.ToInt32(txtNum.Text);

            while (results.Count < numofpubs)
            {
                Database route = new Database();
                foreach (Database routeList in pubs)
                {
                    if ((routeList.score > route.score) && (!results.Contains(routeList)))
                    {
                        route = routeList;
                    }
                }

                //If not enough matches are found to satisfy the user's request, then a message is displayed to them
                if (route.name == null)
                {
                    MessageBox.Show("Not enough matches. Found " + results.Count + " matches.");
                    break;
                }
                results.Add(route);
            }

            //Plots a pushpin on the Map for each Recomended pub
            foreach (Database routeList in results)
            {
                Pushpin pub = new Pushpin();
                pub.Location = new GeoCoordinate(routeList.latitude, routeList.longitude);
                pub.Content = routeList.name;
                routeMap.Children.Add(pub);
            }                                         
           
        }
   }
}