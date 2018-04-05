using Newtonsoft.Json.Linq;
using Semantics3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_sem
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {



                Device_PropertyEntities db = new Device_PropertyEntities();

                Products products = new Products("SEM3B172108EEFABB2C0B0B9AB0D8308228D",
                                         "Mzg5MTg2YjAxY2M4YmI3MTRiOTNjMzczZjNmYmFhYjI");

                //products.products_field("cat_id", "9359");//Phones 
                //products.products_field("cat_id", "19299");//Tablets 
                products.products_field("cat_id", "12855");//Laptops
                                                          

                string SiteName = (from n in db.ListSite
                                   where
                                   (n.IsUsing != true)
                                   select n.site).FirstOrDefault();

                if (SiteName == null || SiteName == "") break;

                products.products_field("site", SiteName);

                var c2 = 0;
                var cont = 0;
                long? max = 0;
                // View the results of the query
                JObject hashResponse = products.get_products();
                while (true)
                {
                    if ((int)hashResponse["results_count"] > 0)
                    {
                        JArray arrayProducts = (JArray)hashResponse["results"];

                        // For each product in the results
                        for (int i = 0; i < arrayProducts.Count; i++)
                        {

                            JArray ecommerceStores = (JArray)arrayProducts[i]["sitedetails"];
                            for (int k = 0; k < ecommerceStores.Count; k++)
                            {
                                String StoreDomen = (String)ecommerceStores[k]["name"];

                                long? IdDom = (from n in db.ListSite
                                               where
                                               (n.site == StoreDomen)
                                               select n.ID).FirstOrDefault();

                                if (IdDom == 0)
                                {
                                    ListSite tem = new ListSite
                                    {
                                        site = StoreDomen,
                                        IsUsing = false
                                    };
                                    db.ListSite.Add(tem);
                                    db.SaveChanges();

                                }

                            }




                            var L = new Laptops();

                            String productSem3_id = (String)arrayProducts[i]["sem3_id"];
                            L.sem3_id = productSem3_id;

                            String productUPC = (String)arrayProducts[i]["upc"];
                            L.upc = productUPC;

                            String productEAN = (String)arrayProducts[i]["ean"];
                            L.ean = productEAN;

                            JArray gtinsArray = (JArray)arrayProducts[i]["gtins"];
                            if (gtinsArray != null)
                            {
                                for (int k = 0; k < gtinsArray.Count; k++)
                                {
                                    L.gtinsArray += "[" + gtinsArray[k] + "]";
                                }
                            }

                            String productCat_id = (String)arrayProducts[i]["cat_id"];
                            L.cat_id = productCat_id;


                            String productCategory = (String)arrayProducts[i]["category"];
                            L.Category = productCategory;

                            String productName = (String)arrayProducts[i]["name"];
                            L.Name = productName;


                            String productBrand = (String)arrayProducts[i]["brand"];
                            L.Brand = productBrand;

                            String productManufacturer = (String)arrayProducts[i]["manufacturer"];
                            L.Manufacturer = productManufacturer;

                            String productModel = (String)arrayProducts[i]["model"];
                            L.Model = productModel;

                            String productmpn = (String)arrayProducts[i]["mpn"];
                            L.mpn = productmpn;


                            String productvar = (String)arrayProducts[i]["variation_id"];
                            L.variation_id = productvar;

                            JArray variation_secondaryids = (JArray)arrayProducts[i]["variation_secondaryids"];
                            if (variation_secondaryids != null)
                            {
                                for (int k = 0; k < variation_secondaryids.Count; k++)
                                {
                                    L.variation_id_Array += "[" + variation_secondaryids[k] + "]";
                                }
                            }

                            String productWeight = (String)arrayProducts[i]["weight"];
                            L.Weight = productWeight;
                            String productWidth = (String)arrayProducts[i]["width"];
                            L.Physical_width = productWidth;
                            String productLength = (String)arrayProducts[i]["length"];
                            L.Physical_length = productLength;
                            String productHeight = (String)arrayProducts[i]["height"];
                            L.Physical_height = productHeight;

                            String productColor = (String)arrayProducts[i]["color"];
                            L.Color = productColor;

                            lbl0:
                            long? IdLap = (from n in db.Laptops
                                           where
                                           (n.sem3_id == L.sem3_id)
                                           select n.ID).FirstOrDefault();
                            if (IdLap == 0)
                            {
                                db.Laptops.Add(L);
                                db.SaveChanges();
                                goto lbl0;
                            }


                            JObject featuresArray = (JObject)arrayProducts[i]["features"];
                            try
                            {
                                c2 = 0;
                                foreach (var f in featuresArray)
                                {
                                    var key = f.Key.ToString();
                                    var val = f.Value.ToString();
                                    if (key == "" || val == "")
                                        continue;
                                    c2++;
                                    lbl1:
                                    long? IdPT = (from n in db.PropertyType
                                                  where
                                                  (n.Name == key)
                                                  select n.ID).FirstOrDefault();
                                    if (IdPT == 0)
                                    {
                                        var PT = new PropertyType
                                        {
                                            Name = key
                                        };
                                        db.PropertyType.Add(PT);
                                        db.SaveChanges();

                                        goto lbl1;
                                    }

                                    lbl2:
                                    long? IdPV = (from n in db.PropertyValue
                                                  where
                                                  ((n.PTid == IdPT) && (n.Value == val))
                                                  select n.ID).FirstOrDefault();
                                    if (IdPV == 0)
                                    {
                                        var PV = new PropertyValue
                                        {
                                            Value = val,
                                            PTid = IdPT
                                        };
                                        db.PropertyValue.Add(PV);
                                        db.SaveChanges();
                                        goto lbl2;
                                    }
                                    var temp = new PropertyValueToLaptops();
                                    temp.LapId = IdLap;
                                    temp.PVId = IdPV;
                                    db.PropertyValueToLaptops.Add(temp);
                                    db.SaveChanges();

                                }
                                cont++;
                                Console.WriteLine(cont + " - Device - " + IdLap + "(" + c2 + ") --- " + SiteName);
                                max = IdLap > max ? IdLap : max;

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message + "Device - " + IdLap);
                            }

                        }
                    }

                    hashResponse = products.iterate_products();
                    if (hashResponse == null)
                    {
                        hashResponse = products.iterate_products();
                        if (hashResponse == null)
                        {
                            var site = db.ListSite
                            .Where(c => c.site == SiteName)
                            .FirstOrDefault();

                            // Внести изменения
                            site.IsUsing = true;
                            site.TotalResult = cont.ToString();
                            site.LastDeviceId = max.ToString();
                            // Сохранить изменения
                            db.SaveChanges();
                            break;
                        }
                    }
                }
            }
            Console.ReadKey();
            //}
        }
    }
}



/*
 try
                        {
                            foreach (var f in featuresArray)
                            {
                                switch (f.Key.ToString())
                                {
                                    case "Interface":

                                        L.Interface += f.Value.ToString() + ";";//Mini-VGA
                                        break;
//********************************************Product************************************************
                                    case "Product Color":
                                        L.Product_Color += f.Value.ToString() + ";";//Black
                                        break;
                                    case "Notebook Type":
                                        L.Processor_Type += f.Value.ToString() + ";";//Ultrabook
                                        break;
                                    case "Product Line":
                                    case "Series":

                                        L.Product_Line += f.Value.ToString() + ";";//ASUS TAICHI 21
                                        break;
//********************************************Common************************************************

                                    case "Wireless Protocol":
                                    case "Data Link Protocol":
                                    case "Wireless LAN Standard":
                                    case "Wireless Networking":
                                    case "WLAN":

                                        L.Wireless_Protocol += f.Value.ToString() + ";";//802.11n , Bluetooth 4.0
                                        break;


                                    case "HDMI":
                                        L.HDMI += f.Value.ToString() + ";";//Yes
                                        break;
                                    case "Webcam":
                                        L.WebCam += f.Value.ToString() + ";";//Yes
                                        break;

                                    case "Ethernet Technology":
                                    case "Wired Protoco":
                                    case "LAN":
                                    

                                        L.Ethernet += f.Value.ToString() + ";";//Gigabit Ethernet
                                        break;

                                    case "Bluetooth Standard":
                                        L.Bluetooth_Standard += f.Value.ToString() + ";";//4.0
                                        break;

                                    case "Wi-Fi":
                                    case "Wi-Fi Standard":
                                    case "Wireless Type":

                                        L.WiFi_Standard += f.Value.ToString() + ";";//yes or 802.11n
                                        break;

                                    case "Optical Drive":
                                    case "Optical Media Supported":
                                    case "Optical Drive Interface":
                                    case "Optical Drive Type":

                                        L.Optical_Drive_Type += f.Value.ToString() + ";";// DVDA±RW / CD - RW
                                        break;

                                    case "Digital Media Reader or Slots":
                                    case "Card Reader":
                                    case "Memory Card Reader":
                                    case "Media Card Reader":

                                        L.CardReader += f.Value.ToString() + ";";// Yes
                                        break;
                                    case "Total Number of USB Ports":
                                    case "Number of USB 2.0 Ports":

                                        L.Total_USB += f.Value.ToString() + ";";//3
                                        break;
                                    case "Dimensions (W x D x H)":
                                    case "Item Dimensions L x W x H":

                                        L.Dimensions += f.Value.ToString() + ";";//3
                                        break;
                                    case "Integrated Microphone":
                                    case "Speakers":

                                        L.Micro += f.Value.ToString() + ";";//Yes
                                        break;
                                    case "Audio Line Out":
                                    case "Audio Ports":
                                    case "Audio-out Ports":
                                    case "Audio-out Ports (#)":

                                        L.AudiOut += f.Value.ToString() + ";";//Yes
                                        break;

                                    case "Battery Energy":
                                    case "Power Provided":

                                        L.Battery_Energy += f.Value.ToString() + ";";//44 Wh
                                        break;
//********************************************Processor************************************************
                                    case "Processor":
                                    case "CPU":
                                    case "CPU Support":


                                        L.Processor += f.Value.ToString() + ";";//IntelA® Corea?? i3 
                                        break;

                                    case "Processor Number":
                                    case "Processor Model":
                                    case "Processor Type":
                                    case "CPU Type":


                                        L.Processor_Type += f.Value.ToString() + ";";//I5-3317U
                                        break;
                                    case "Cache":
                                    case "Cache Memory":
                                        L.Processor_Cache += f.Value.ToString() + ";";//3 MB
                                        break;

                                    case "Processor Manufacturer":
                                    case "Chipset Manufacturer":
                                    case "Processor Brand":


                                        L.Processor_Manufacturer += f.Value.ToString() + ";";//Intel
                                        break;

                                    case "Clock Speed":
                                    case "Processor Speed":
                                    case "CPU Speed":

                                        L.Processor_Speed += f.Value.ToString() + ";";//1.7 GHz
                                        break;
                                    case "Chipset Type":
                                    case "Chipset Model":
                                    case "Chipset":

                                        L.Processor_Chipset += f.Value.ToString() + ";";//Mobile Intel QS77 Express
                                        break;
                                    case "Graphics Processor Vendor":
                                    case "Graphics Controller Model":
                                    case "Graphics":
                                    case "Graphics Chip":
                                    case "GPU/VPU":
                                    case "Graphics Card":
                                    case "Graphics Coprocessor":

                                        L.Processor_Graphics += f.Value.ToString() + ";";//Intel HD Graphics 4000
                                        break;

//********************************************RAM************************************************
                                    case "Installed Size":
                                    case "Standard Memory":
                                    case "System Memory (RAM)":
                                    case "System Memory (RAM) Expandable To":
                                    case "RAM":

                                        L.RAM_Value += f.Value.ToString() + ";";//4 GB
                                        break;
                                    case "Technology":
                                    case "Memory Technology":
                                    case "Type of Memory (RAM)":
                                    case "Memory Standard":
                                    case "Computer Memory Type":

                                        L.RAM_Type += f.Value.ToString() + ";";//DDR3 SDRAM
                                        break;
                                    case "Memory Speed":

                                        L.RAM_Speed += f.Value.ToString() + ";";//1600 MHz
                                        break;


//********************************************HDD************************************************
                                    case "Capacity":
                                    case "Drive Capacity":  
                                    case "Hard Drive Capacity":
                                    case "Computer Hard Drive Size":
                                    case "Solid State Drive Capacity":
                                    case "Storage":
                                    case "HDD":
                                    case "Hard Drive Size":


                                        L.HDD_Value += f.Value.ToString() + ";";//128 GB
                                        break;
                                    case "Hard Drive":
                                    case "Serial ATA Interface":
                                    case "HDD Interface":
                                    case "Hard Drive Type":
                                    case "Hard Drive Interface":


                                        L.HDD_Drive += f.Value.ToString() + ";";//SATA
                                        break;
                                    case "Spindle Speed":
                                    case "HDD RPM":
                                    case "Hard Drive Rotational Speed":
                                    case "Hard Drive RPM":
                                    

                                        L.HDD_Speed += f.Value.ToString() + ";";//1600 MHz
                                        break;



//********************************************Screen************************************************
                                    case "Diagonal Size (metric)":

                                        L.Screen_Size_Metric += f.Value.ToString() + ";";//29.5 cm
                                        break;
                                    case "Image Aspect Ratio":
                                    case "Aspect Ratio":

                                        L.Screen_Relation += f.Value.ToString() + ";";//16:9
                                        break;
                                    case "Max Resolution":
                                    case "Screen Resolution":

                                        L.Screen_Resolutions += f.Value.ToString() + ";";//1920 x 1080
                                        break;
                                    case "Screen Size":
                                    case "Diagonal Size":


                                        L.Screen_Size_inches += f.Value.ToString() + ";";//15.6"
                                        break;
                                    case "TFT Technology":
                                    case "Display Type":
                                    case "Display Screen Technology":
                                    case "Display Screen Type":

                                        L.Screen_Type += f.Value.ToString() + ";";//IPS
                                        break;





//********************************************Blob************************************************
                                    case "blob":

                                        L.DescriptionEng += f.Value.ToString() + ";";//Desc
                                        break;

//********************************************OS************************************************
                                    case "Family":
                                    case "Operating System":
                                    case "Operating System Platform":

                                        L.Operating_System += f.Value.ToString() + ";";//Windows 8
                                        break;


//********************************************Other************************************************
                                    default:
                                        L.Other += "[" + f.Key.ToString() + ":" + f.Value.ToString() + "];";

                                        break;
                                }
                            }
                            db.Laptop.Add(L);
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message + "Device - " + L.upc);
                            db.NewLaptop.Add(L);
                            db.SaveChanges();
                        }
     */
