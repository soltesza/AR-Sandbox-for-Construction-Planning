using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SLS.Widgets.Table {
  public class DataTableData {

    public class Population {
      public string city { get; set; }

      public string country { get; set; }

      public int rank { get; set; }

      public int population { get; set; }

      public int density { get; set; }

      public float sqkm { get; set; }

      public string extraText { get; set; }

      public int iconIndex { get; set; }

      public Population(string city, string country, int rank, int pop, int den, float sqkm) {
        this.city = city;
        this.country = country;
        this.rank = rank;
        this.population = pop;
        this.density = den;
        this.sqkm = sqkm;
        this.iconIndex = Random.Range(0, 5);
      }
    }

    public static List<Population> Generate() {
      List<Population> plist = new List<Population>();
      plist.Add(new Population("Shanghai", " China", 1, 24150000, 3809, 6340.5f));
      plist.Add(new Population("Karachi", " Pakistan", 2, 23500000, 6663, 3527.0f));
      plist.Add(new Population("Lagos", " Nigeria", 3, 21324000, 18206, 1171.28f));
      plist.Add(new Population("Delhi", " India", 4, 16787941, 11320, 1483.0f));
      plist.Add(new Population("Istanbul", " Turkey", 5, 14377019, 2633, 5461.0f));
      plist.Add(new Population("Tokyo", " Japan", 6, 13297629, 6075, 2189.0f));
      plist.Add(new Population("Tianjin", " China", 7, 12938224, 2314, 4037.0f));
      plist.Add(new Population("Guangzhou", " China", 8, 12700800, 4722, 2089.53f));
      plist.Add(new Population("Mumbai", " India", 9, 12478447, 20680, 603.4f));
      plist.Add(new Population("Moscow", " Russia", 10, 12197596, 4859, 2510.12f));
      plist.Add(new Population("São Paulo", " Brazil", 11, 11895893, 7821, 1521.11f));
      plist.Add(new Population("Beijing", " China", 12, 11716620, 8563, 1368.3f));
      plist.Add(new Population("Shenzhen", " China", 13, 10467400, 5256, 1991.64f));
      plist.Add(new Population("Seoul", " South Korea", 14, 10388055, 17164, 605.21f));
      plist.Add(new Population("Lahore", " Pakistan", 15, 10052000, 5673, 1772.0f));
      plist.Add(new Population("Jakarta", " Indonesia", 16, 9988329, 15040, 664.12f));
      plist.Add(new Population("Kinshasa", " Democratic Republic of the Congo", 17, 9735000, 8710, 1117.62f));
      plist.Add(new Population("Cairo", " Egypt", 18, 9278441, 3008, 3085.1f));
      plist.Add(new Population("Mexico City", " Mexico", 19, 8874724, 5974, 1485.49f));
      plist.Add(new Population("Lima", " Peru", 20, 8693387, 3253, 2672.3f));
      plist.Add(new Population("New York City", " United States", 21, 8491079, 10833, 783.84f));
      plist.Add(new Population("Bengaluru", " India", 22, 8425970, 11876, 709.5f));
      plist.Add(new Population("London", " United Kingdom", 23, 8416500, 5362, 1572.15f));
      plist.Add(new Population("Bangkok", " Thailand", 24, 8280925, 5279, 1568.74f));
      plist.Add(new Population("Dongguan", " China", 25, 8220207, 3329, 2469.4f));
      plist.Add(new Population("Chongqing", " China", 25, 8189800, 1496, 5473.0f));
      plist.Add(new Population("Nanjing", " China", 26, 8187828, 1737, 4713.85f));
      plist.Add(new Population("Tehran", " Iran", 27, 8154051, 11886, 686.0f));
      plist.Add(new Population("Shenyang", " China", 28, 8106171, 626, 12942.0f));
      plist.Add(new Population("Bogotá", " Colombia", 29, 7776845, 9052, 859.11f));
      plist.Add(new Population("Ho Chi Minh City", " Vietnam", 30, 7681700, 3666, 2095.6f));
      plist.Add(new Population("Ningbo", " China", 31, 7605689, 775, 9816.23f));
      plist.Add(new Population("Hong Kong", " China", 32, 7219700, 6537, 1104.43f));
      plist.Add(new Population("Baghdad", " Iraq", 33, 7180889, 1576, 4555.0f));
      plist.Add(new Population("Changsha", " China", 34, 7044118, 596, 11819.0f));
      plist.Add(new Population("Dhaka", " Bangladesh", 35, 6970105, 45307, 153.84f));
      plist.Add(new Population("Wuhan", " China", 36, 6886253, 5187, 1327.61f));
      plist.Add(new Population("Hyderabad", " India", 37, 6809970, 10958, 621.48f));
      plist.Add(new Population("Hanoi", " Vietnam", 38, 6844100, 2059, 3323.6f));
      plist.Add(new Population("Faisalabad", " Pakistan", 39, 6480765, 27145, 237.0f));
      plist.Add(new Population("Rio de Janeiro", " Brazil", 39, 6429923, 5357, 1200.27f));
      plist.Add(new Population("Foshan", " China", 40, 6151622, 3023, 2034.62f));
      plist.Add(new Population("Santiago", " Chile", 41, 5743719, 4595, 1249.9f));
      plist.Add(new Population("Riyadh", " Saudi Arabia", 42, 5676621, 4600, 1233.98f));
      plist.Add(new Population("Ahmedabad", " India", 43, 5570585, 11728, 475.0f));
      plist.Add(new Population("Singapore", " Singapore", 44, 5399200, 7579, 712.4f));
      plist.Add(new Population("Shantou", " China", 45, 5391028, 2611, 2064.42f));
      plist.Add(new Population("Yangon", " Myanmar", 46, 5214000, 8708, 598.75f));
      plist.Add(new Population("Saint Petersburg", " Russia", 47, 5191690, 3608, 1439.0f));
      plist.Add(new Population("Chennai", " India", 50, 4792949, 11238, 426.51f));
      plist.Add(new Population("Abidjan", " Ivory Coast", 51, 4765000, 2249, 2119.0f));
      plist.Add(new Population("Chengdu", " China", 52, 4741929, 11263, 421.0f));
      plist.Add(new Population("Alexandria", " Egypt", 53, 4616625, 2007, 2300.0f));
      plist.Add(new Population("Kolkata", " India", 54, 4486679, 22355, 200.7f));
      plist.Add(new Population("Ankara", " Turkey", 48, 4470800, 2340, 1910.92f));
      plist.Add(new Population("Xi'an", " China", 55, 4467837, 5369, 832.17f));
      plist.Add(new Population("Surat", " India", 56, 4462002, 13666, 326.515f));
      plist.Add(new Population("Johannesburg", " South Africa", 57, 4434827, 2696, 1644.98f));
      plist.Add(new Population("Dar es Salaam", " Tanzania", 58, 4364541, 2676, 1631.12f));
      plist.Add(new Population("Suzhou", " China", 59, 4327066, 2623, 1649.72f));
      plist.Add(new Population("Harbin", " China", 60, 4280701, 2491, 1718.2f));
      plist.Add(new Population("Giza", " Egypt", 61, 4239988, 14667, 289.08f));
      plist.Add(new Population("Zhengzhou", " China", 63, 4122087, 4059, 1015.66f));
      plist.Add(new Population("New Taipei City", " Taiwan", 64, 3954929, 1927, 2052.57f));
      plist.Add(new Population("Los Angeles", " United States", 65, 3884307, 3200, 1213.85f));
      plist.Add(new Population("Cape Town", " South Africa", 66, 3740026, 1530, 2444.97f));
      plist.Add(new Population("Yokohama", " Japan", 67, 3680267, 8414, 437.38f));
      plist.Add(new Population("Busan", " South Korea", 68, 3590101, 4686, 766.12f));
      plist.Add(new Population("Hangzhou", " China", 69, 3560391, 4889, 728.19f));
      plist.Add(new Population("Xiamen", " China", 70, 3531347, 2078, 1699.0f));
      plist.Add(new Population("Quanzhou", " China", 71, 3520846, 3315, 1062.0f));
      plist.Add(new Population("Berlin", " Germany", 72, 3517424, 3944, 891.75f));
      plist.Add(new Population("Rawalpindi", " Pakistan", 72, 3510000, 27638, 127.0f));
      plist.Add(new Population("Jeddah", " Saudi Arabia", 73, 3456259, 1958, 1765.0f));
      plist.Add(new Population("Durban", " South Africa", 74, 3442361, 1502, 2291.31f));
      plist.Add(new Population("Hyderabad", " Pakistan", 75, 3429471, 30083, 114.0f));
      plist.Add(new Population("Kabul", " Afghanistan", 76, 3414100, 12415, 275.0f));
      plist.Add(new Population("Casablanca", " Morocco", 77, 3359818, 17168, 195.7f));
      plist.Add(new Population("Hefei", " China", 78, 3352076, 3998, 838.52f));
      plist.Add(new Population("Pyongyang", " North Korea", 79, 3255388, 1541, 2113.0f));
      plist.Add(new Population("Madrid", " Spain", 80, 3207247, 5294, 605.77f));
      plist.Add(new Population("Peshawar", " Pakistan", 80, 3201000, 25608, 125.0f));
      plist.Add(new Population("Ekurhuleni", " South Africa", 81, 3178470, 1609, 1975.31f));
      plist.Add(new Population("Nairobi", " Kenya", 82, 3138369, 4829, 694.9f));
      plist.Add(new Population("Zhongshan", " China", 83, 3121275, 1750, 1783.67f));
      plist.Add(new Population("Pune", " India", 84, 3115431, 6913, 450.69f));
      plist.Add(new Population("Addis Ababa", " Ethiopia", 85, 3103673, 5889, 526.99f));
      plist.Add(new Population("Jaipur", " India", 86, 3073350, 6337, 485.0f));
      plist.Add(new Population("Buenos Aires", " Argentina", 87, 3054300, 15046, 203.0f));
      plist.Add(new Population("Wenzhou", " China", 88, 3039439, 2559, 1187.88f));

      // Add some sample 'extra text'
      for(int i = 0; i < plist.Count; i++) {
        DataTableData.Population p = plist[i];
        if(p.city == "Seoul") {
          p.extraText =
            "Seoul (서울 – officially the Seoul Special City – is the capital and largest metropolis of South Korea, forming the heart of the Seoul Capital Area, which includes the surrounding Incheon metropolis and Gyeonggi province, the world's second largest metropolitan area with over 25.6 million people.";
        }
        if(p.city == "Tokyo") {
          p.extraText =
            "Officially Tokyo Metropolis (東京都 Tōkyō-to), is one of the 47 prefectures of Japan, and is both the capital and largest city of Japan.";
        }
        if(p.city == "Delhi") {
          p.extraText = "Delhi, officially the National Capital Territory of Delhi, is the capital territory of India.";
        }
        if(p.city == "Shanghai") {
          p.extraText =
            "Shanghai is the largest Chinese city by population and the largest city proper by population in the world.  It is a global financial center, and a transport hub with the world's busiest container port.";
        }
      }

      return plist;
    }


  }

}