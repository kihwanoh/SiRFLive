﻿namespace GPSUtils
{
    using System;
    using System.Runtime.InteropServices;

    public class GPSUtilsClass
    {
        private const double a83 = 6378137.0;
        private const double b83 = 6356752.3142;
        private const double e2 = 0.00669438006861;
        private const double ep2 = 0.00673949682181;
        private const double eps83 = 0.00673949675703;
        private const double es83 = 0.0066943800047;
        private const double PI = 3.14159265359;
        private const double RadiusA = 6378137.0;
        private const double RadiusB = 6356752.314;
        public const double SOL = 299792458.0;
        public const double ToDEC = 57.29577951307855;
        public const double ToRads = 0.017453292519944444;

        public static double ComputeApproxAltitude(double[] svPos)
        {
            double num = 6371009.0;
            return (Math.Sqrt(((svPos[0] * svPos[0]) + (svPos[1] * svPos[1])) + (svPos[2] * svPos[2])) - num);
        }

        public static bool ComputeAzEl(double[] navPos, double[] svPos, out double Az, out double El)
        {
            double num;
            double num2;
            double num3;
            float num10;
            ConvertXYZ2GEO(navPos[0], navPos[1], navPos[2], out num, out num2, out num3);
            double[][] numArray = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                numArray[i] = new double[3];
            }
            double num4 = Math.Sin(num);
            double num5 = Math.Cos(num);
            double num6 = Math.Sin(num2);
            double num7 = Math.Cos(num2);
            numArray[0][0] = -num7 * num4;
            numArray[0][1] = -num6 * num4;
            numArray[0][2] = num5;
            numArray[1][0] = -num6;
            numArray[1][1] = num7;
            numArray[1][2] = 0.0;
            numArray[2][0] = -num7 * num5;
            numArray[2][1] = -num6 * num5;
            numArray[2][2] = -num4;
            double[] numArray2 = new double[3];
            double[] numArray3 = new double[3];
            double[] numArray4 = new double[3];
            numArray2[0] = svPos[0] - navPos[0];
            numArray2[1] = svPos[1] - navPos[1];
            numArray2[2] = svPos[2] - navPos[2];
            double num12 = Math.Sqrt(((numArray2[0] * numArray2[0]) + (numArray2[1] * numArray2[1])) + (numArray2[2] * numArray2[2]));
            numArray4[0] = numArray2[0] / num12;
            numArray4[1] = numArray2[1] / num12;
            numArray4[2] = numArray2[2] / num12;
            for (short j = 0; j < 3; j = (short) (j + 1))
            {
                numArray3[j] = ((numArray[j][0] * numArray4[0]) + (numArray[j][1] * numArray4[1])) + (numArray[j][2] * numArray4[2]);
            }
            numArray3[2] = Math.Max(-1.0, Math.Min(numArray3[2], 1.0));
            double num11 = Math.Asin(-numArray3[2]);
            float num9 = (float) Math.Max(0.0, num11);
            float num13 = (float) Math.Cos((double) num9);
            if (num13 > 0.0)
            {
                double num1 = numArray3[1] / ((double) num13);
                num10 = (float) (numArray3[0] / ((double) num13));
            }
            else
            {
                num10 = 1f;
            }
            Az = Math.Acos((double) num10);
            El = num9;
            num9 /= 3.141593f;
            return true;
        }

        public int ComputeZone(double Lat, double Lng)
        {
            double num2 = 6.0;
            int num = (int) (Lng * (180.0 / (4.0 * Math.Atan(1.0))));
            if (num >= 0)
            {
                return (int) ((30.0 + (((double) num) / num2)) + 1.0);
            }
            return (int) ((30.0 - (((double) Math.Abs(num)) / num2)) + 1.0);
        }

        public void ConvertGEO2UTM(double Lat, double Lng, int Zone, out double N, out double E)
        {
            double num20 = 6.0;
            if (Lat == 0.0 && Lng == 0.0)
            {
                N = 0.0;
                E = 0.0;
            }
            else
            {
                double num19;
                if (Zone <= 30)
                {
                    num19 = (((Zone - 30) * ((int) num20)) - (((int) num20) / 2)) * 0.017453292519944444;
                }
                else
                {
                    num19 = (((Zone - 30) * ((int) num20)) + (((int) num20) / 2)) * 0.017453292519944444;
                }
                double x = num19 - Lng;
                double num16 = 6378137.0;
                double num17 = 6356752.3142;
                double num18 = 0.00673949675703;
                double num9 = Math.Pow(num16, 2.0) / num17;
                double num3 = 1.0 - ((0.75 * num18) * (1.0 - ((0.9375 * num18) * (1.0 - ((0.97222222222222221 * num18) * (1.0 - ((0.984375 * num18) * (1.0 - (0.99 * num18)))))))));
                double num4 = (0.75 * num18) * (1.0 - ((1.5625 * num18) * (1.0 - ((1.2833333333333334 * num18) * (1.0 - ((1.1889204545454546 * num18) * (1.0 - (1.1413978494623656 * num18))))))));
                double num5 = (0.625 * num18) * (1.0 - ((0.96527777777777779 * num18) * (1.0 - ((0.97751798561151082 * num18) * (1.0 - (0.98402905550444653 * num18))))));
                double num6 = (0.4861111111111111 * Math.Pow(num18, 2.0)) * (1.0 - ((1.953125 * num18) * (1.0 - (1.4737933333333333 * num18))));
                double num7 = (0.41015625 * Math.Pow(num18, 3.0)) * (1.0 - (2.9475 * num18));
                double num8 = 0.3609375 * Math.Pow(num18, 4.0);
                double num2 = ((num3 * num9) * Lat) - ((((num4 * num9) * Math.Sin(Lat)) * Math.Cos(Lat)) * ((((1.0 + (num5 * Math.Pow(Math.Sin(Lat), 2.0))) + (num6 * Math.Pow(Math.Sin(Lat), 4.0))) + (num7 * Math.Pow(Math.Sin(Lat), 6.0))) + (num8 * Math.Pow(Math.Sin(Lat), 8.0))));
                double num10 = num9 * Math.Pow(Math.Pow(1.0 / Math.Cos(Lat), 2.0) + num18, -0.5);
                double num11 = (0.5 * num10) * Math.Sin(Lat);
                double num12 = (0.16666666666666666 * num10) * ((-1.0 + (2.0 * Math.Pow(Math.Cos(Lat), 2.0))) + (num18 * Math.Pow(Math.Cos(Lat), 4.0)));
                double num13 = (0.083333333333333329 * num11) * (((-1.0 + (6.0 * Math.Pow(Math.Cos(Lat), 2.0))) + ((9.0 * num18) * Math.Pow(Math.Cos(Lat), 4.0))) + ((4.0 * Math.Pow(num18, 2.0)) * Math.Pow(Math.Cos(Lat), 6.0)));
                double num14 = (0.0083333333333333332 * num10) * (((1.0 - (20.0 * Math.Pow(Math.Cos(Lat), 2.0))) + ((24.0 - (58.0 * num18)) * Math.Pow(Math.Cos(Lat), 4.0))) + ((72.0 * num18) * Math.Pow(Math.Cos(Lat), 6.0)));
                double num15 = (0.0027777777777777779 * num11) * ((1.0 - (60.0 * Math.Pow(Math.Cos(Lat), 2.0))) + (120.0 * Math.Pow(Math.Cos(Lat), 4.0)));
                N = 0.9996 * (((num2 + (num11 * Math.Pow(x, 2.0))) + (num13 * Math.Pow(x, 4.0))) + (num15 * Math.Pow(x, 6.0)));
                E = 500000.0 - (0.9996 * (((num10 * x) + (num12 * Math.Pow(x, 3.0))) + (num14 * Math.Pow(x, 5.0))));
            }
        }

        public static void ConvertGEO2XYZ(double Lat, double Lng, double Ht, out double Px, out double Py, out double Pz)
        {
            double num2 = 40680631590769;
            double num3 = 40408299981544.359;
            double num = num2 / Math.Pow((num2 * Math.Pow(Math.Cos(Lat), 2.0)) + (num3 * Math.Pow(Math.Sin(Lat), 2.0)), 0.5);
            Px = ((num + Ht) * Math.Cos(Lat)) * Math.Cos(Lng);
            Py = ((num + Ht) * Math.Cos(Lat)) * Math.Sin(Lng);
            Pz = (((num3 / num2) * num) + Ht) * Math.Sin(Lat);
        }

        public void ConvertUTM2GEO(double Northing, double Easting, int Zone, out double Latitude, out double Longitude)
        {
            double num18;
            double[] numArray = new double[6];
            double[] numArray2 = new double[5];
            int index = 0;
            double num19 = 6.0;
            if (Zone <= 30)
            {
                num18 = (((Zone - 30) * ((int) num19)) - (((int) num19) / 2)) * 0.017453292519944444;
            }
            else
            {
                num18 = (((Zone - 30) * ((int) num19)) + (((int) num19) / 2)) * 0.017453292519944444;
            }
            Northing /= 0.9996;
            Easting = (Easting - 500000.0) / 0.9996;
            double x = 6378137.0;
            double num16 = 6356752.3142;
            double num17 = 0.00673949675703;
            double num8 = Math.Pow(x, 2.0) / num16;
            double num = 1.0 - ((0.75 * num17) * (1.0 - ((0.9375 * num17) * (1.0 - ((0.97222222222222221 * num17) * (1.0 - ((0.984375 * num17) * (1.0 - (0.99 * num17)))))))));
            double num9 = (0.75 * num17) * (1.0 - ((1.5625 * num17) * (1.0 - ((1.2833333333333334 * num17) * (1.0 - ((1.1889204545454546 * num17) * (1.0 - (1.1413978494623656 * num17))))))));
            double num10 = (0.625 * num17) * (1.0 - ((0.96527777777777779 * num17) * (1.0 - ((0.97751798561151082 * num17) * (1.0 - (0.98402905550444653 * num17))))));
            double num11 = (0.4861111111111111 * Math.Pow(num17, 2.0)) * (1.0 - ((1.953125 * num17) * (1.0 - (1.4737933333333333 * num17))));
            double num12 = (0.41015625 * Math.Pow(num17, 3.0)) * (1.0 - (2.9475 * num17));
            double num13 = 0.3609375 * Math.Pow(num17, 4.0);
            numArray[0] = 0.0;
            numArray2[0] = 0.0;
            index = 0;
            do
            {
                index++;
                numArray[index] = numArray[index - 1] + (((Northing - numArray2[index - 1]) / num) / num8);
                numArray2[index] = ((num * num8) * numArray[index]) - ((((num9 * num8) * Math.Sin(numArray[index])) * Math.Cos(numArray[index])) * ((((1.0 + (num10 * Math.Pow(Math.Sin(numArray[index]), 2.0))) + (num11 * Math.Pow(Math.Sin(numArray[index]), 4.0))) + (num12 * Math.Pow(Math.Sin(numArray[index]), 6.0))) + (num13 * Math.Pow(Math.Sin(numArray[index]), 8.0))));
            }
            while (Math.Abs((double) (Northing - numArray2[index])) > 0.002);
            double num2 = 1.0 / (num8 * Math.Pow(Math.Pow(1.0 / Math.Cos(numArray[index]), 2.0) + num17, -0.5));
            double num3 = (((-0.5 * Math.Pow(num2, 2.0)) * Math.Sin(numArray[index])) * Math.Cos(numArray[index])) * (1.0 + (num17 * Math.Pow(Math.Cos(numArray[index]), 2.0)));
            double num4 = (-0.16666666666666666 * Math.Pow(num2, 3.0)) * ((2.0 - Math.Pow(Math.Cos(numArray[index]), 2.0)) + (num17 * Math.Pow(Math.Cos(numArray[index]), 4.0)));
            double num5 = ((-0.083333333333333329 * Math.Pow(num2, 2.0)) * num3) * (((3.0 + ((2.0 - (9.0 * num17)) * Math.Pow(Math.Cos(numArray[index]), 2.0))) + ((10.0 * num17) * Math.Pow(Math.Cos(numArray[index]), 4.0))) - ((4.0 * Math.Pow(num17, 2.0)) * Math.Pow(Math.Cos(numArray[index]), 6.0)));
            double num6 = (0.0083333333333333332 * Math.Pow(num2, 5.0)) * (((24.0 - (20.0 * Math.Pow(Math.Cos(numArray[index]), 2.0))) + ((1.0 + (8.0 * num17)) * Math.Pow(Math.Cos(numArray[index]), 4.0))) - ((2.0 * num17) * Math.Pow(Math.Cos(numArray[index]), 6.0)));
            double num7 = ((0.0027777777777777779 * Math.Pow(num2, 4.0)) * num3) * (45.0 + (16.0 * Math.Pow(Math.Cos(numArray[index]), 4.0)));
            Latitude = ((numArray[index] + (num3 * Math.Pow(Easting, 2.0))) + (num5 * Math.Pow(Easting, 4.0))) + (num7 * Math.Pow(Easting, 6.0));
            Longitude = ((num18 + (num2 * Easting)) + (num4 * Math.Pow(Easting, 3.0))) + (num6 * Math.Pow(Easting, 5.0));
            Latitude *= 57.29577951307855;
            if (num18 < 0.0)
            {
                Longitude *= -57.29577951307855;
            }
            else
            {
                Longitude *= 57.29577951307855;
            }
        }

        public void ConvertVelocityGEO2LTP(double Speed, double Heading, double Climb, out double Evel, out double Nvel, out double Dvel)
        {
            Evel = Speed * Math.Sin((3.14159265359 * Heading) / 180.0);
            Nvel = Speed * Math.Cos((3.14159265359 * Heading) / 180.0);
            Dvel = -Climb;
        }

        public void ConvertVelocityLTP2GEO(double Evel, double Nvel, double Dvel, out double Speed, out double Heading, out double Climb)
        {
            Climb = -Dvel;
            Speed = Math.Pow(Math.Pow(Nvel, 2.0) + Math.Pow(Evel, 2.0), 0.5);
            Heading = Math.Atan2(Evel, Nvel);
            if (Heading < 0.0)
            {
                Heading += 6.28318530718;
            }
        }

        public void ConvertVelocityXYZ2LTP(double Xvel, double Yvel, double Zvel, double GEOLat, double GEOLng, out double Evel, out double Nvel, out double Dvel)
        {
            Nvel = ((Xvel * (-Math.Sin(GEOLat) * Math.Cos(GEOLng))) + (Yvel * (-Math.Sin(GEOLat) * Math.Sin(GEOLng)))) + (Zvel * Math.Cos(GEOLat));
            Evel = (Xvel * -Math.Sin(GEOLng)) + (Yvel * Math.Cos(GEOLng));
            Dvel = ((Xvel * (-Math.Cos(GEOLat) * Math.Cos(GEOLng))) + (Yvel * (-Math.Cos(GEOLat) * Math.Sin(GEOLng)))) + (Zvel * -Math.Sin(GEOLat));
        }

        public static void ConvertXYZ2GEO(double Px, double Py, double Pz, out double Lat, out double Lng, out double Ht)
        {
            if ((Px == 0.0) && (Py == 0.0))
            {
                Lat = 0.0;
                Lng = 0.0;
                Ht = 0.0;
            }
            else
            {
                double num3 = Math.Pow(Math.Pow(Px, 2.0) + Math.Pow(Py, 2.0), 0.5);
                double a = Math.Atan2(Pz * 6378137.0, num3 * 6356752.314);
                Lat = Math.Atan2(Pz + (42841.312017236363 * Math.Pow(Math.Sin(a), 3.0)), num3 - (42697.673207663982 * Math.Pow(Math.Cos(a), 3.0)));
                Lng = Math.Atan2(Py, Px);
                double num = Math.Pow(6378137.0, 2.0) / Math.Pow((Math.Pow(6378137.0, 2.0) * Math.Pow(Math.Cos(Lat), 2.0)) + (Math.Pow(6356752.314, 2.0) * Math.Pow(Math.Sin(Lat), 2.0)), 0.5);
                Ht = (num3 / Math.Cos(Lat)) - num;
            }
        }

        public static double DOTPROD(double[] a, double[] b)
        {
            return (((a[0] * b[0]) + (a[1] * b[1])) + (a[2] * b[2]));
        }

        public static double getDistance(double[] a, double[] b)
        {
            double d = Math.Pow(a[0] - b[0], 2.0) + Math.Pow(a[1] - b[1], 2.0);
            d += Math.Pow(a[2] - b[2], 2.0);
            return Math.Sqrt(d);
        }

        public double[] LinearInterpolateXYZ(double[] loc1, double[] loc2, double percent)
        {
            double[] numArray = new double[3];
            double num = loc1[0] - loc2[0];
            double num2 = loc1[1] - loc2[1];
            double num3 = loc1[2] - loc2[2];
            numArray[0] = loc1[0] - ((num * percent) / 100.0);
            numArray[1] = loc1[1] - ((num2 * percent) / 100.0);
            numArray[2] = loc1[2] - ((num3 * percent) / 100.0);
            return numArray;
        }

        public double PointDistance(double Lat1, double Lon1, double Lat2, double Lon2)
        {
            return Math.Pow(Math.Pow(Lat2 - Lat1, 2.0) + Math.Pow(Lon2 - Lon1, 2.0), 0.5);
        }

        public bool PointsEqual(double Lat1, double Lon1, double Lat2, double Lon2, double dTolerence)
        {
            return (this.PointDistance(Lat1, Lon1, Lat2, Lon2) < dTolerence);
        }
    }
}

