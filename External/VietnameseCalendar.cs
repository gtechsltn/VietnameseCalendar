/*
 * Copyright 2004 Ho Ngoc Duc [http://come.to/duc]. All Rights Reserved.
 * <p>
 * Permission to use, copy, modify, and redistribute this software and its
 * documentation for personal, non-commercial use is hereby granted provided that
 * this copyright notice appears in all copies.
 * </p>
 */
using System;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32;

namespace System.Globalization
{
    /// <summary>
    /// Represents time in divisions, such as months, days, and years. Years are calculated using the
    /// Vietnamese calendar, while days and months are calculated using the lunisolar calendar.
    /// </summary>
    [Serializable()]
    public class VietnameseCalendar : Calendar
    {
        #region Constants

        //// TODO: Must use Environment.GetResourceString()
        private static readonly Resources.ResourceManager resource =
            new Resources.ResourceManager("mscorlib", Reflection.Assembly.GetAssembly(typeof(int)));

        /**
         * Each gregorian years will be ecrypted with 3 bytes (24 bits).
         * - First 7 bits (hight): number of days between Western New Year and Lunar New Year days (1/1).
         * - 1 bit next: is 0 if the leap month has 29 days, is 1 if 30 days.
         * - 12 bits next: represents number of days for each 12 months (same as bit 8).
         * - Last 4 bits (low): is 0 if the lunar year is not a leap one, otherwise, it represents the leap month (ex: 4 means Apr).
         *
         * The following table contains 400 gregorian year-codes from 1800 to 2199.
         */
        private static readonly int[] yearsCode = new int[]
        {
            //// 19th Century (1800-1899)
            0x30baa3, 0x56ab50, 0x422ba0, 0x2cab61, 0x52a370, 0x3c51e8, 0x60d160, 0x4ae4b0, 0x376926, 0x58daa0,
            0x445b50, 0x3116d2, 0x562ae0, 0x3ea2e0, 0x28e2d2, 0x4ec950, 0x38d556, 0x5cb520, 0x46b690, 0x325da4,
            0x5855d0, 0x4225d0, 0x2ca5b3, 0x52a2b0, 0x3da8b7, 0x60a950, 0x4ab4a0, 0x35b2a5, 0x5aad50, 0x4455b0,
            0x302b74, 0x562570, 0x4052f9, 0x6452b0, 0x4e6950, 0x386d56, 0x5e5aa0, 0x46ab50, 0x3256d4, 0x584ae0,
            0x42a570, 0x2d4553, 0x50d2a0, 0x3be8a7, 0x60d550, 0x4a5aa0, 0x34ada5, 0x5a95d0, 0x464ae0, 0x2eaab4,
            0x54a4d0, 0x3ed2b8, 0x64b290, 0x4cb550, 0x385757, 0x5e2da0, 0x4895d0, 0x324d75, 0x5849b0, 0x42a4b0,
            0x2da4b3, 0x506a90, 0x3aad98, 0x606b50, 0x4c2b60, 0x359365, 0x5a9370, 0x464970, 0x306964, 0x52e4a0,
            0x3cea6a, 0x62da90, 0x4e5ad0, 0x392ad6, 0x5e2ae0, 0x4892e0, 0x32cad5, 0x56c950, 0x40d4a0, 0x2bd4a3,
            0x50b690, 0x3a57a7, 0x6055b0, 0x4c25d0, 0x3695b5, 0x5a92b0, 0x44a950, 0x2ed954, 0x54b4a0, 0x3cb550,
            0x286b52, 0x4e55b0, 0x3a2776, 0x5e2570, 0x4852b0, 0x32aaa5, 0x56e950, 0x406aa0, 0x2abaa3, 0x50ab50,
            //// 20th Century (1900-1999)
            0x3c4bd8, 0x624ae0, 0x4ca570, 0x3854d5, 0x5cd260, 0x44d950, 0x315554, 0x5656a0, 0x409ad0, 0x2a55d2,
            0x504ae0, 0x3aa5b6, 0x60a4d0, 0x48d250, 0x33d255, 0x58b540, 0x42d6a0, 0x2cada2, 0x5295b0, 0x3f4977,
            0x644970, 0x4ca4b0, 0x36b4b5, 0x5c6a50, 0x466d50, 0x312b54, 0x562b60, 0x409570, 0x2c52f2, 0x504970,
            0x3a6566, 0x5ed4a0, 0x48ea50, 0x336a95, 0x585ad0, 0x442b60, 0x2f86e3, 0x5292e0, 0x3dc8d7, 0x62c950,
            0x4cd4a0, 0x35d8a6, 0x5ab550, 0x4656a0, 0x31a5b4, 0x5625d0, 0x4092d0, 0x2ad2b2, 0x50a950, 0x38b557,
            0x5e6ca0, 0x48b550, 0x355355, 0x584da0, 0x42a5b0, 0x2f4573, 0x5452b0, 0x3ca9a8, 0x60e950, 0x4c6aa0,
            0x36aea6, 0x5aab50, 0x464b60, 0x30aae4, 0x56a570, 0x405260, 0x28f263, 0x4ed940, 0x38db47, 0x5cd6a0,
            0x4896d0, 0x344dd5, 0x5a4ad0, 0x42a4d0, 0x2cd4b4, 0x52b250, 0x3cd558, 0x60b540, 0x4ab5a0, 0x3755a6,
            0x5c95b0, 0x4649b0, 0x30a974, 0x56a4b0, 0x40aa50, 0x29aa52, 0x4e6d20, 0x39ad47, 0x5eab60, 0x489370,
            0x344af5, 0x5a4970, 0x4464b0, 0x2c74a3, 0x50ea50, 0x3d6a58, 0x6256a0, 0x4aaad0, 0x3696d5, 0x5c92e0,
            //// 21st Century (2000-2099)
            0x46c960, 0x2ed954, 0x54d4a0, 0x3eda50, 0x2a7552, 0x4e56a0, 0x38a7a7, 0x5ea5d0, 0x4a92b0, 0x32aab5,
            0x58a950, 0x42b4a0, 0x2cbaa4, 0x50ad50, 0x3c55d9, 0x624ba0, 0x4ca5b0, 0x375176, 0x5c5270, 0x466930,
            0x307934, 0x546aa0, 0x3ead50, 0x2a5b52, 0x504b60, 0x38a6e6, 0x5ea4e0, 0x48d260, 0x32ea65, 0x56d520,
            0x40daa0, 0x2d56a3, 0x5256d0, 0x3c4afb, 0x6249d0, 0x4ca4d0, 0x37d0b6, 0x5ab250, 0x44b520, 0x2edd25,
            0x54b5a0, 0x3e55d0, 0x2a55b2, 0x5049b0, 0x3aa577, 0x5ea4b0, 0x48aa50, 0x33b255, 0x586d20, 0x40ad60,
            0x2d4b63, 0x525370, 0x3e49e8, 0x60c970, 0x4c54b0, 0x3768a6, 0x5ada50, 0x445aa0, 0x2fa6a4, 0x54aad0,
            0x4052e0, 0x28d2e3, 0x4ec950, 0x38d557, 0x5ed4a0, 0x46d950, 0x325d55, 0x5856a0, 0x42a6d0, 0x2c55d4,
            0x5252b0, 0x3ca9b8, 0x62a930, 0x4ab490, 0x34b6a6, 0x5aad50, 0x4655a0, 0x2eab64, 0x54a570, 0x4052b0,
            0x2ab173, 0x4e6930, 0x386b37, 0x5e6aa0, 0x48ad50, 0x332ad5, 0x582b60, 0x42a570, 0x2e52e4, 0x50d160,
            0x3ae958, 0x60d520, 0x4ada90, 0x355aa6, 0x5a56d0, 0x462ae0, 0x30a9d4, 0x54a2d0, 0x3ed150, 0x28e952,
            //// 22nd Century (2100-2199)
            0x4eb520, 0x38d727, 0x5eada0, 0x4a55b0, 0x362db5, 0x5a45b0, 0x44a2b0, 0x2eb2b4, 0x54a950, 0x3cb559,
            0x626b20, 0x4cad50, 0x385766, 0x5c5370, 0x484570, 0x326574, 0x5852b0, 0x406950, 0x2a7953, 0x505aa0,
            0x3baaa7, 0x5ea6d0, 0x4a4ae0, 0x35a2e5, 0x5aa550, 0x42d2a0, 0x2de2a4, 0x52d550, 0x3e5abb, 0x6256a0,
            0x4c96d0, 0x3949b6, 0x5e4ab0, 0x46a8d0, 0x30d4b5, 0x56b290, 0x40b550, 0x2a6d52, 0x504da0, 0x3b9567,
            0x609570, 0x4a49b0, 0x34a975, 0x5a64b0, 0x446a90, 0x2cba94, 0x526b50, 0x3e2b60, 0x28ab61, 0x4c9570,
            0x384ae6, 0x5cd160, 0x46e4a0, 0x2eed25, 0x54da90, 0x405b50, 0x2c36d3, 0x502ae0, 0x3a93d7, 0x6092d0,
            0x4ac950, 0x32d556, 0x58b4a0, 0x42b690, 0x2e5d94, 0x5255b0, 0x3e25fa, 0x6425b0, 0x4e92b0, 0x36aab6,
            0x5c6950, 0x4674a0, 0x31b2a5, 0x54ad50, 0x4055a0, 0x2aab73, 0x522570, 0x3a5377, 0x6052b0, 0x4a6950,
            0x346d56, 0x585aa0, 0x42ab50, 0x2e56d4, 0x544ae0, 0x3ca570, 0x2864d2, 0x4cd260, 0x36eaa6, 0x5ad550,
            0x465aa0, 0x30ada5, 0x5695d0, 0x404ad0, 0x2aa9b3, 0x50a4d0, 0x3ad2b7, 0x5eb250, 0x48b540, 0x33d556
        };

        #endregion

        #region String tables

        private static readonly string[] selestialStems = new string[]
        {
            "Giáp", "Ất", "Bính", "Đinh", "Mậu", "Kỷ", "Canh", "Tân", "Nhâm", "Quý"
        };

        private static readonly string[] terrestrialBranches = new string[]
        {
            "Tý", "Sửu", "Dần", "Mão", "Thìn", "Tỵ", "Ngọ", "Mùi", "Thân", "Dậu", "Tuất", "Hợi"
        };

        private static readonly string[] monthSpeechNames = new string[]
        {
            "Giêng", "Hai", "Ba", "Tư", "Năm", "Sáu", "Bảy", "Tám", "Chín", "Mười", "Một", "Chạp"
        };

        private static readonly string[] propitiousHour = new string[]
        {
            "110100101100", "001101001011", "110011010010", "101100110100", "001011001101", "010010110011"
        };

        private static readonly string[] minorSolarTerms = new string[]
        {
            "Xuân Phân", "Thanh Minh", "Cốc Vũ",      "Lập Hạ",   "Tiểu Mãn",   "Mang Chủng",
            "Hạ Chí",    "Tiểu Thử",   "Đại Thử",     "Lập Thu",  "Xử Thử",     "Bạch Lộ",
            "Thu Phân",  "Hàn Lộ",     "Sương Giáng", "Lập Đông", "Tiểu Tuyết", "Đại Tuyết",
            "Đông Chí",  "Tiểu Hàn",   "Đại Hàn",     "Lập Xuân", "Vũ Thủy",    "Kinh Trập"
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the current calendar is solar-based, lunar-based,
        /// or a combination of both.
        /// </summary>
        /// <returns>This property always returns the <see cref="CalendarAlgorithmType.LunisolarCalendar"/>
        /// value.</returns>
        public override CalendarAlgorithmType AlgorithmType
        {
            get { return CalendarAlgorithmType.LunisolarCalendar; }
        }

        /// <summary>
        /// Specifies the era that corresponds to the current VietnameseCalendar object.
        /// </summary>
        public const int VietnameseEra = 1;

        /// <summary>
        /// Gets the eras that correspond to the range of dates and times supported by the current
        /// VietnameseCalendar object.
        /// </summary>
        /// <returns>An array of 32-bit signed integers that specify the relevant eras. The return value
        /// for a VietnameseCalendar object is always an array containing one element equal to
        /// the <see cref="VietnameseEra"/> value.</returns>
        [ComVisible(false)]
        public override int[] Eras
        {
            get { return new int[] { VietnameseEra }; }
        }

        /// <summary>
        /// Lunar New Year 1800.
        /// </summary>
        private static readonly DateTime minDate = new DateTime(1800, 1, 25);

        /// <summary>
        /// Gets the minimum date and time supported by the VietnameseCalendar class.</summary>
        /// <returns>A <see cref="DateTime"/> object that represents January 25, 1800
        /// in the Gregorian calendar, which is equivalent to the constructor DateTime(1800, 1, 25).</returns>
        [ComVisible(false)]
        public override DateTime MinSupportedDateTime
        {
            get { return minDate; }
        }

        private static readonly DateTime maxDate = new DateTime(2199, 12, 31, 23, 59, 59, 999);

        /// <summary>
        /// Gets the maximum date and time supported by the VietnameseCalendar class.</summary>
        /// <returns>A <see cref="DateTime"/> object that represents the last moment on December 31, 2199
        /// in the Gregorian calendar, which is approximately equal to the constructor DateTime(2199, 12, 31).</returns>
        [ComVisible(false)]
        public override DateTime MaxSupportedDateTime
        {
            get { return maxDate; }
        }

        /// <summary>
        /// Gets or sets the last year of a 100-year range that can be represented by a 2-digit year.
        /// </summary>
        /// <returns>The last year of a 100-year range that can be represented by a 2-digit year.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The value in a set operation is less than 99
        /// or greater than the maximum supported year in the current calendar.</exception>
        /// <exception cref="InvalidOperationException">The current VietnameseCalendar object is read-only.</exception>
        public override int TwoDigitYearMax
        {
            get
            {
                if (base.TwoDigitYearMax < 0)
                {
                    //// Call Win32 ::GetCalendarInfo() to retrieve CAL_ITWODIGITYEARMAX value
                    base.TwoDigitYearMax = GetSystemTwoDigitYearSetting(
                        1,      // CAL_GREGORIAN - Gregorian (localized)
                        this.GetYear(maxDate));
                }

                return base.TwoDigitYearMax;
            }
            set
            {
                this.VerifyWritable();

                if (value < MinCalendarYear || value > MaxCalendarYear)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.CurrentCulture,
                            resource.GetString("ArgumentOutOfRange_Range"),
                            MinCalendarYear,
                            MaxCalendarYear));
                }

                base.TwoDigitYearMax = value;
            }
        }

        internal static int MinCalendarYear { get { return minDate.Year; } }

        internal static int MaxCalendarYear { get { return maxDate.Year; } }

        #endregion

        #region Native methods

        /// <summary>
        /// Retrieves information about a calendar for a locale specified by identifier.
        /// </summary>
        /// <param name="localeId">Locale identifier that specifies the locale for which to retrieve
        /// calendar information.</param>
        /// <param name="calendarId">Calendar identifier.</param>
        /// <param name="calendarType">Type of information to retrieve.</param>
        /// <param name="data">Pointer to a buffer in which this function retrieves the requested data
        /// as a string. If <c>CAL_RETURN_NUMBER</c> is specified in <paramref name="calendarType"/>,
        /// this parameter must be set to a <c>null</c> pointer.</param>
        /// <param name="dataSize">Size, in characters, of the <paramref name="data"/> buffer.
        /// The application can set this parameter to 0 to return the required size for the calendar data
        /// buffer. In this case, the <paramref name="data"/> parameter is not used.
        /// If <c>CAL_RETURN_NUMBER</c> is specified for <paramref name="calendarType"/>,
        /// the value of <paramref name="dataSize"/> must be <c>0</c>.</param>
        /// <param name="value">Pointer to a variable that receives the requested data as a number.
        /// If <c>CAL_RETURN_NUMBER</c> is not specified in <paramref name="calendarType"/>,
        /// then <paramref name="value"/> must be <c>null</c>.</param>
        /// <returns>
        /// <p>The number of characters retrieved in the <paramref name="data"/> buffer,
        /// with <paramref name="dataSize"/> set to a nonzero value, if successful.<br/>
        /// If the function succeeds, <paramref name="dataSize"/> is set to 0, and <c>CAL_RETURN_NUMBER</c>
        /// is not specified, the return value is the size of the buffer required to hold the
        /// calendar information.<br/>
        /// If the function succeeds, <paramref name="dataSize"/> is set 0, and <c>CAL_RETURN_NUMBER</c>
        /// is specified, the return value is the size of the value retrieved in <paramref name="value"/>,
        /// that is, 2 for the Unicode version of the function or 4 for the ANSI version.</p>
        /// <p>This function returns 0 if it does not succeed.</p>
        /// </returns>
        [DllImport("Kernel32.dll", EntryPoint = "GetCalendarInfo", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int GetCalendarInfo(
            int/*LCID*/ localeId,
            int/*CALID*/ calendarId,
            int/*CALTYPE*/ calendarType,
            StringBuilder/*LPSTR*/ data,
            int dataSize,
            ref int/*LPDWORD*/ value);

        internal static int GetTwoDigitYearMax(int calendarId)
        {
            try
            {
                int num = 0, ret = GetCalendarInfo(
                    0x042a,     // vi-VN
                    calendarId,
                    0x20000030, // CAL_RETURN_NUMBER | CAL_ITWODIGITYEARMAX
                    null,
                    0,
                    ref num);

                if (ret < 1)
                {
                    Console.WriteLine("Native function Kernel32.GetCalendarInfo() returns 0." + Environment.NewLine
                        + "Call GetLastError() to get extended error information." + Environment.NewLine
                        + "GetLastError() can return one of the following error codes:" + Environment.NewLine
                        + " * ERROR_INSUFFICIENT_BUFFER" + Environment.NewLine
                        + " * ERROR_INVALID_FLAGS" + Environment.NewLine
                        + " * ERROR_INVALID_PARAMETER");

                    return -1;
                }

                return num;
            }
            catch (Exception e)
            {
                Console.WriteLine("Native function Kernel32.GetCalendarInfo() calling exception: {0}", e);
            }

            return int.MinValue;
        }

        #endregion

        #region Internal methods

        internal static int GetSystemTwoDigitYearSetting(int calendarId, int defaultYearValue)
        {
            int num = GetTwoDigitYearMax(calendarId);
            if (num < 0)
            {
                RegistryKey key = null;
                try
                {
                    key = Registry.CurrentUser.OpenSubKey(@"Control Panel\International\Calendars\TwoDigitYearMax", false);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (ArgumentException)
                {
                }

                if (key != null)
                {
                    try
                    {
                        object obj = key.GetValue(calendarId.ToString(CultureInfo.InvariantCulture));
                        if (obj != null)
                        {
                            try
                            {
                                num = int.Parse(obj.ToString(), CultureInfo.InvariantCulture);
                            }
                            catch (ArgumentException)
                            {
                            }
                            catch (FormatException)
                            {
                            }
                            catch (OverflowException)
                            {
                            }
                        }
                    }
                    finally
                    {
                        key.Close();
                    }
                }

                if (num < 0)
                {
                    num = defaultYearValue;
                }
            }

            return num;
        }

        internal void VerifyWritable()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(resource.GetString("InvalidOperation_ReadOnly"));
            }
        }

        internal static void CheckTicksRange(long ticks)
        {
            if (ticks < minDate.Ticks || ticks > maxDate.Ticks)
            {
                throw new ArgumentOutOfRangeException(
                    "time",
                    string.Format(
                        CultureInfo.CurrentCulture,
                        resource.GetString("ArgumentOutOfRange_CalendarRange"),
                        minDate,
                        maxDate));
            }
        }

        internal static void CheckEraRange(int era)
        {
            if (era != 0 && era != VietnameseEra)
            {
                throw new ArgumentOutOfRangeException(
                    "era",
                    resource.GetString("ArgumentOutOfRange_InvalidEraValue"));
            }
        }

        internal static void CheckYearRange(int year)
        {
            if (year < MinCalendarYear || year > MaxCalendarYear)
            {
                throw new ArgumentOutOfRangeException(
                    "year",
                    string.Format(
                        CultureInfo.CurrentCulture,
                        resource.GetString("ArgumentOutOfRange_Range"),
                        MinCalendarYear,
                        MaxCalendarYear));
            }
        }

        internal static void CheckMonthRange(int month, int leapMonth)
        {
            if (month < 1 || month > 13 || (month == 13 && leapMonth == 0))
            {
                throw new ArgumentOutOfRangeException(
                    "month",
                    resource.GetString("ArgumentOutOfRange_Month"));
            }
        }

        #endregion

        #region Helper methods

        private static int GetYearCode(int year)
        {
            return yearsCode[year - 1800];
        }

        /// <summary>
        /// Returns a date that represents the Lunar New Year's Day in the specified solar year.
        /// </summary>
        private static DateTime GetLunarNewYear(int year, DateTimeKind copied)
        {
            //// Solar New Year's Day (Gregorian calendar)
            DateTime date = new DateTime(year, 1, 1, 0, 0, 0, copied);

            //// Offset of Tet (Lunar New Year's Day)
            return date.AddDays(GetYearCode(year) >> 17);
        }

        /// <summary>
        /// Returns a date that represents the Lunar New Year's Day in the specified solar date.
        /// </summary>
        private static DateTime GetLunarNewYear(DateTime time)
        {
            DateTime date = GetLunarNewYear(time.Year, time.Kind);
            if (date > time) date = GetLunarNewYear(time.Year - 1, time.Kind);

            return date;
        }

        /// <summary>
        /// Returns the leap month of the specified lunisolar year.
        /// </summary>
        private static int GetLunarLeapMonth(int year)
        {
            CheckYearRange(year);
            int leapMonth = GetYearCode(year) & 0xf;

            return (leapMonth > 0) ? (leapMonth + 1) : 0;
        }

        /// <summary>
        /// Returns number of days in specified month of the year.
        /// </summary>
        /// <param name="year">A gregorian year.</param>
        /// <param name="month">An integer from 1 through 13 that represents the month.</param>
        /// <param name="leapMonth">The leap month number (if exists).</param>
        private static int GetMonthLength(int year, int month, int leapMonth)
        {
            if (leapMonth > 0)
            {
                if (month == leapMonth) month = 0;
                else if (month > leapMonth) month--;
            }

            return (((GetYearCode(year) >> (16 - month)) & 0x1) == 0) ? 29 : 30;
        }

        #endregion

        #region Implement Calendar methods

        /// <summary>
        /// Retrieves the era that corresponds to the specified <see cref="DateTime"/> object.
        /// </summary>
        /// <returns>An integer that represents the era in the time parameter.</returns>
        /// <param name="time">The <see cref="DateTime"/> object to read.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time is less than
        /// <see cref="MinSupportedDateTime"/> or greater than <see cref="MaxSupportedDateTime"/>.</exception>
        [ComVisible(false)]
        public override int GetEra(DateTime time)
        {
            CheckTicksRange(time.Ticks);

            return VietnameseEra;
        }

        /// <summary>
        /// Returns the year in the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> object to read.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time is less than
        /// <see cref="MinSupportedDateTime"/> or greater than <see cref="MaxSupportedDateTime"/>.</exception>
        /// <returns>An integer that represents the year in the specified <see cref="DateTime"/> object.</returns>
        public override int GetYear(DateTime time)
        {
            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.GetYear(), >> time = '{0}'", time);

            CheckTicksRange(time.Ticks);
            DateTime date = GetLunarNewYear(time.Year, time.Kind);

            return (date > time) ? (time.Year - 1) : time.Year;
        }

        /// <summary>
        /// Returns the month in the specified date.
        /// </summary>
        /// <returns>An integer from 1 to 13 that represents the month specified in the time parameter,
        /// </returns>
        /// <param name="time">The <see cref="DateTime"/> object to read.</param>
        public override int GetMonth(DateTime time)
        {
            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.GetMonth(), >> time = '{0}'", time);

            int year, month, day;
            FromDateTime(time, out year, out month, out day);

            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.GetMonth(), << month = {0}", month);
            return month;
        }

        /// <summary>
        /// Calculates the day of the month in the specified date.
        /// </summary>
        /// <returns>An integer from 1 through 30 that represents the day of the month
        /// specified in the time parameter.</returns>
        /// <param name="time">The <see cref="DateTime"/> object to read.</param>
        public override int GetDayOfMonth(DateTime time)
        {
            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.GetDayOfMonth(), >> time = '{0}'", time);

            int year, month, day;
            FromDateTime(time, out year, out month, out day);

            return day;
        }

        /// <summary>
        /// Calculates the day of the week in the specified date.
        /// </summary>
        /// <returns>One of the <see cref="DayOfWeek"/> values that represents the day of the week
        /// specified in the time parameter.</returns>
        /// <param name="time">The <see cref="DateTime"/> object to read.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time is less than
        /// <see cref="MinSupportedDateTime"/> or greater than <see cref="MaxSupportedDateTime"/>.</exception>
        public override DayOfWeek GetDayOfWeek(DateTime time)
        {
            CheckTicksRange(time.Ticks);

            return (DayOfWeek)((int)Math.Floor((double)time.Ticks / 864000000000L + 1) % 7);
        }

        /// <summary>
        /// Calculates the day of the year in the specified date.
        /// </summary>
        /// <returns>An integer from 1 through 354 in a common year, or 1 through 384 in a leap year,
        /// that represents the day of the year specified in the time parameter.</returns>
        /// <param name="time">The <see cref="DateTime"/> object to read.</param>
        public override int GetDayOfYear(DateTime time)
        {
            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.GetDayOfYear(), >> time = '{0}'", time);

            int year, month, day, leapMonth;
            FromDateTime(time, out year, out month, out day, out leapMonth);

            for (int i = 1; i < month; i++)
            {
                day += GetMonthLength(year, i, leapMonth);
            }

            return day;
        }

        /// <summary>
        /// Determines whether the specified year in the specified era is a leap year.
        /// </summary>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <returns>True if the specified year is a leap year; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Year or era is outside the range
        /// supported by this calendar.</exception>
        public override bool IsLeapYear(int year, int era)
        {
            return (this.GetLeapMonth(year, era) > 0);
        }

        /// <summary>
        /// Determines whether the specified month in the specified year and era is a leap month.
        /// </summary>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="month">An integer from 1 through 13 that represents the month.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <returns>True if the month parameter is a leap month; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Year, month, or era is outside the range
        /// supported by this calendar.</exception>
        public override bool IsLeapMonth(int year, int month, int era)
        {
            int leapMonth = this.GetLeapMonth(year, era);
            CheckMonthRange(month, leapMonth);

            return (leapMonth > 0 && month == leapMonth);
        }

        /// <summary>
        /// Determines whether the specified date in the specified era is a leap day.
        /// </summary>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="month">An integer from 1 through 13 that represents the month.</param>
        /// <param name="day">An integer from 1 through 30 that represents the day.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <returns>True if the specified day is a leap day; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Year, month, day, or era is outside
        /// the range supported by this calendar.</exception>
        public override bool IsLeapDay(int year, int month, int day, int era)
        {
            int leapMonth = this.GetLeapMonth(year, era);
            CheckMonthRange(month, leapMonth);

            int daysInMonth = GetMonthLength(year, month, leapMonth);
            if (day < 1 || day > daysInMonth)
            {
                throw new ArgumentOutOfRangeException(
                    "day",
                    string.Format(CultureInfo.CurrentCulture, resource.GetString("ArgumentOutOfRange_Day"), daysInMonth, month));
            }

            return (leapMonth > 0 && month == leapMonth);
        }

        /// <summary>
        /// Calculates the number of months in the specified year and era.
        /// </summary>
        /// <returns>The number of months in the specified year in the specified era.
        /// The return value is 12 months in a common year or 13 months in a leap year.</returns>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <exception cref="ArgumentOutOfRangeException">Year or era is outside the range supported
        /// by this calendar.</exception>
        public override int GetMonthsInYear(int year, int era)
        {
            return this.IsLeapYear(year, era) ? 13 : 12;
        }

        /// <summary>
        /// Calculates the number of days in the specified month of the specified year and era.
        /// </summary>
        /// <returns>The number of days in the specified month of the specified year and era.</returns>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="month">An integer from 1 through 12 in a common year,
        /// or 1 through 13 in a leap year, that represents the month.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <exception cref="ArgumentOutOfRangeException">Year, month, or era is outside the range
        /// supported by this calendar.</exception>
        public override int GetDaysInMonth(int year, int month, int era)
        {
            int leapMonth = this.GetLeapMonth(year, era);
            CheckMonthRange(month, leapMonth);

            return GetMonthLength(year, month, leapMonth);
        }

        /// <summary>
        /// Calculates the number of days in the specified year and era.
        /// </summary>
        /// <returns>The number of days in the specified year and era.</returns>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <exception cref="ArgumentOutOfRangeException">Year or era is outside the range supported
        /// by this calendar.</exception>
        public override int GetDaysInYear(int year, int era)
        {
            int leapMonth = this.GetLeapMonth(year, era),
                count = 0,
                len = (leapMonth > 0) ? 13 : 12;

            for (int month = 1; month <= len; month++)
            {
                count += GetMonthLength(year, month, leapMonth);
            }

            return count;
        }

        /// <summary>
        /// Calculates the date that is the specified number of months away from the specified date.
        /// </summary>
        /// <returns>A new <see cref="DateTime"/> object that results from adding the specified
        /// number of months to the time parameter.</returns>
        /// <param name="months">The number of months to add.</param>
        /// <param name="time">The <see cref="DateTime"/> object to add months to.</param>
        /// <exception cref="ArgumentOutOfRangeException">Months is less than -120000 or greater than 120000.
        /// -or- time is less than <see cref="MinSupportedDateTime"/> or greater than
        /// <see cref="MaxSupportedDateTime"/>.</exception>
        /// <exception cref="ArgumentException">The result is outside the supported range of a
        /// <see cref="DateTime"/> object.</exception>
        public override DateTime AddMonths(DateTime time, int months)
        {
            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.AddMonths(), >> time = '{0}', months = {1}", time, months);
            return time.AddMonths(months);

            /*
            int year, month, day, leapMonth;
            FromDateTime(time, out year, out month, out day, out leapMonth);

            //// Adds months
            month += months;

            int inc;
            while (month < 1 || month > 13 || (month == 13 && leapMonth == 0))
            {
                inc   = (month < 1) ? -1 : 1;

                //// Re-calculates the lunar year
                year += inc;

                leapMonth = this.GetLeapMonth(year, 0);
                month    += ((leapMonth > 0) ? 13 : 12) * (-inc);
            }

            //// Decrease the lunar day when overload
            int daysInMonth = GetMonthLength(year, month, leapMonth);
            if (day > daysInMonth) day = daysInMonth;

            //// Converts back to solar date (Gregorian)
            DateTime date = new DateTime(year, month, day, time.Hour, time.Minute, time.Second, time.Millisecond, this);

            return date;
            */
        }

        /// <summary>
        /// Calculates the date that is the specified number of years away from the specified date.
        /// </summary>
        /// <returns>A new <see cref="DateTime"/> object that results from adding the specified number
        /// of years to the time parameter.</returns>
        /// <param name="time">The <see cref="DateTime"/> object to add years to.</param>
        /// <param name="years">The number of years to add.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time is less than
        /// <see cref="MinSupportedDateTime"/> or greater than <see cref="MaxSupportedDateTime"/>.</exception>
        /// <exception cref="ArgumentException">The result is outside the supported range of a
        /// <see cref="DateTime"/> object.</exception>
        public override DateTime AddYears(DateTime time, int years)
        {
            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.AddYears(), >> time = '{0}', years = {1}", time, years);
            return time.AddYears(years);

            /*
            //// Converts to lunar date
            int year, month, day, leapMonth;
            FromDateTime(time, out year, out month, out day, out leapMonth);

            year += years;
            CheckYearRange(year);

            //// Re-calculates the lunar month
            leapMonth = this.GetLeapMonth(year, 0);
            if (month > 12 && leapMonth == 0) month = 12;

            //// Decreases the lunar day when overload
            int daysInMonth = GetMonthLength(year, month, leapMonth);
            if (day > daysInMonth) day = daysInMonth;

            //// Converts back to solar date (Gregorian)
            DateTime date = new DateTime(year, month, day, time.Hour, time.Minute, time.Second, time.Millisecond, this);

            return date;
            */
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> object that is set to the specified date, time, and era.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> object that is set to the specified date, time, and era.
        /// </returns>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="month">An integer from 1 through 13 that represents the month.</param>
        /// <param name="millisecond">An integer from 0 through 999 that represents the millisecond.</param>
        /// <param name="day">An integer from 1 through 30 that represents the day.</param>
        /// <param name="minute">An integer from 0 through 59 that represents the minute.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="hour">An integer from 0 through 23 that represents the hour.</param>
        /// <param name="second">An integer from 0 through 59 that represents the second.</param>
        /// <exception cref="ArgumentOutOfRangeException">Year, month, day, hour, minute, second,
        /// millisecond, or era is outside the range supported by this calendar.</exception>
        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
        {
            //// DEBUG
            ////Console.WriteLine(
            ////    "VietnameseCalendar.ToDateTime(), >> year = {0}, month = {1}, day = {2}, hour = {3}, minute = {4}, second = {5}, millisecond = {6}, era = {7}",
            ////    year, month, day, hour, minute, second, millisecond, era);

            //// Validate parameters
            int leapMonth = this.GetLeapMonth(year, era);
            CheckMonthRange(month, leapMonth);

            int days = GetMonthLength(year, month, leapMonth);
            if (day < 1 || day > days)
            {
                throw new ArgumentOutOfRangeException(
                    "day",
                    string.Format(CultureInfo.CurrentCulture, resource.GetString("ArgumentOutOfRange_Day"), days, month));
            }

            //// Lunar New Year's Day
            DateTime time = GetLunarNewYear(year, DateTimeKind.Unspecified);

            //// Count number of days from the Lunar New Year's Day to the specified lunar-date
            days = day - 1;
            while (--month > 0)
            {
                days += GetMonthLength(year, month, leapMonth);
            }

            //// TODO: Uses Julian day number, converts into DateTime object
            //int jdn = JulianDayNumber(time) + days;

            //// Appends time to the result object
            time = time.AddDays(days + (double)hour / 24 + (double)minute / 1440 + (double)second / 86400 + (double)millisecond / 86400000);

            //// DEBUG
            ////Console.WriteLine("VietnameseCalendar.ToDateTime(), << time = {0}", time);
            return time;
        }

        /// <summary>
        /// Calculates the lunar-date for the specified solar-date.
        /// </summary>
        /// <param name="time">Solar date to convert.</param>
        /// <param name="year">Output lunar-year.</param>
        /// <param name="month">Output lunar-month of the year.</param>
        /// <param name="day">Output lunar-day of the month.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time is outside the range supported by this calendar.</exception>
        public static void FromDateTime(DateTime time, out int year, out int month, out int day)
        {
            int leapMonth;
            FromDateTime(time, out year, out month, out day, out leapMonth);
        }

        /// <summary>
        /// Calculates the lunar-date for the specified solar-date.
        /// </summary>
        /// <param name="time">Solar date to convert.</param>
        /// <param name="year">Output lunar-year.</param>
        /// <param name="month">Output lunar-month of the year.</param>
        /// <param name="day">Output lunar-day of the month.</param>
        /// <param name="leapMonth">Output lunar leap-month -or- zero if not exists.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time is outside the range supported by this calendar.</exception>
        public static void FromDateTime(DateTime time, out int year, out int month, out int day, out int leapMonth)
        {
            CheckTicksRange(time.Ticks);

            //// Calculates the lunar-year
            DateTime date = GetLunarNewYear(time);
            year = date.Year;   // Lunar Year

            //// Gets the Year's Leap month (if it has)
            leapMonth = GetLunarLeapMonth(year);

            //// Number of days from the New Year's Day
            int daysCount = JulianDayNumber(time) - JulianDayNumber(date),
                count = 0, lastLen = 0;

            //// Scans the floor month nearest with the solar-day
            month = 1;
            while (count < daysCount)
            {
                lastLen = GetMonthLength(year, month++, leapMonth);

                count  += lastLen;
            }

            //// Calculates the lunar-day
            day = 1;
            if (count > daysCount)
            {
                month--;    // Lunar Month
                day += daysCount - count + lastLen;
            }

            //// TODO: Exception handling!
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Calculates the leap month for the specified year and era.
        /// </summary>
        /// <returns>A positive integer from 1 through 13 that indicates the leap month in the
        /// specified year and era.
        /// -or-
        /// Zero if this calendar does not support a leap month, or if the year and era parameters
        /// do not specify a leap year.</returns>
        /// <param name="era">An integer that represents the era.</param>
        /// <param name="year">An integer that represents the year.</param>
        /// <exception cref="ArgumentOutOfRangeException">Year or era is outside the range supported
        /// by this calendar.</exception>
        public override int GetLeapMonth(int year, int era)
        {
            CheckEraRange(era);

            return GetLunarLeapMonth(year);
        }

        /// <summary>
        /// Converts the specified two-digit year to a four-digit year.
        /// </summary>
        /// <returns>An integer that contains the four-digit representation of the year parameter.</returns>
        /// <param name="year">A two-digit integer that represents the year to convert.</param>
        /// <exception cref="ArgumentOutOfRangeException">Year is outside the range supported
        /// by this calendar.</exception>
        public override int ToFourDigitYear(int year)
        {
            year = base.ToFourDigitYear(year);
            CheckYearRange(year);

            return year;
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Returns the number of days since 1/1/4713 BC noon.
        /// </summary>
        public static int JulianDayNumber(DateTime time)
        {
            int a = (int)Math.Floor((14.0D - time.Month) / 12);
            int y = time.Year + 4800 - a;
            int m = time.Month + 12 * a - 3;

            int n = time.Day + (int)Math.Floor((153.0D * m + 2) / 5) + (int)Math.Floor(365.25D * y);
            int jdn = n - (int)Math.Floor(0.01D * y) + (int)Math.Floor(0.0025D * y) - 32045;

            //// Is Julius Date ?
            if (jdn < 2299161)  // Julian Day number of 1582/10/15
            {
                jdn = n - 32083;
            }

            //// Add more fractions
            ////double jd = jdn + (double)time.Hour / 24 - 0.5 + (double)time.Minute / 1440
            ////    + (double)time.Second / 86400 + (double)time.Millisecond / 86400000;

            return jdn;
        }

        /// <summary>
        /// Compute the longitude of the sun at any time.
        /// Algorithm from: "Astronomical Algorithms" by Jean Meeus, 1998.
        /// </summary>
        /// <returns>Number of degrees of Sun Longitude.</returns>
        public static double SunLongitude(DateTime dt)
        {
            DateTime ts = dt.ToUniversalTime();
            //// GMT Julian date number
            double jd = JulianDayNumber(ts) + (double)ts.Hour / 24 - 0.5
                + (double)ts.Minute / 1440 + (double)ts.Second / 86400 + (double)ts.Millisecond / 86400000;

            //// Time in Julian centuries from 2000-01-01 12:00:00 GMT
            double t  = (jd - 2451545.0) / 36525;

            double t2 = t * t;
            double dr = Math.PI / 180;  // Degree to radian

            //// Mean anomaly, degree
            double m  = 357.52911D + 35999.05029 * t - 0.0001537 * t2
                - 0.00000048 * t * t2;  // TODO: not in http://www.srrb.noaa.gov/highlights/sunrise/calcdetails.html

            //// Mean longitude, degree
            double l0 = 280.46646D + 36000.76983 * t + 0.0003032 * t2;

            double dl = (1.914602D - 0.004817 * t - 0.000014 * t2) * Math.Sin(dr * m)
                + (0.019993D - 0.000101 * t) * Math.Sin(dr * 2 * m)
                + 0.000289D * Math.Sin(dr * 3 * m);

            //// True longitude, degree
            double l = l0 + dl;

            //// Obtain apparent longitude by correcting for nutation and aberration
            double omega = 125.04D - 1934.136 * t;
            l -= 0.00569D + 0.00478 * Math.Sin(omega * dr);

            //// Normalize to (0, 2*PI)
            l -= (int)Math.Floor(l / 360) * 360;

            //lambda *= dr; // Convert to radians
            return l;
        }

        /// <summary>
        /// Returns a string that represents the Minor Solar Terms of the specified date.
        /// </summary>
        /// <remarks>
        /// Compute the sun segment at start (00:00) of the day with the given integral
        /// Julian day number. The time zone if the time difference between local time and UTC: 7.0
        /// for UTC+7:00. The function returns a number between 0 and 23.
        /// From the day after March equinox and the 1st major term after March equinox, 0 is returned.
        /// After that, return 1, 2, 3 ...
        /// </remarks>
        /// <param name="dt">A gregorian date.</param>
        /// <returns>A Minor Solar Terms string.</returns>
        public static string GetMinorSolarTerms(DateTime dt)
        {
            CheckTicksRange(dt.Ticks);

            double sl = SunLongitude(dt);
            //// TODO: Hard-code some exceptions
            if (sl > 283.980685381734 && sl < 283.998429466957) sl += 1.019314618264603;
            else if (sl > 268.982233589428 && sl < 268.997418971464) sl += 1.017766410571303;
            else if (sl > 238.994360790922 && sl < 238.998815336006) sl += 1.0056392090773516;
            else if (sl > 298.985299986357 && sl < 298.999892901616) sl += 1.014700013642128;
            else if (sl > 313.988180094718 && sl < 313.995561330842) sl += 1.011819905281312;
            else if (sl > 253.986425624963 && sl < 253.997690579273) sl += 1.0135743750361216;
            //// Not needle
            else if (sl > 328.99464285669 && sl < 328.997513050222) sl += 1.0053571433090626;
            //else if (Math.Round(sl, 10) == 343.9994746162) { }
            else if (sl > 343.998197182546 && sl < 343.999951809195) sl += 1.0018028174527066;
            //// ---
            else if ((sl > 194.002942346778 && sl < 194.013760798233)   // && Math.Round(sl, 10) != 194.0128520093)
                || (sl > 179.000208750401 && sl < 179.01862004219)
                || (sl > 164.000901415201 && sl < 164.027376866403)
                || (sl > 149.000992194126 && sl < 149.035876318057)
                || (sl > 134.000122471553 && sl < 134.038649430489)
                || (sl > 119.00248079265 && sl < 119.035483698587)
                || (sl > 104.001461964773 && sl < 104.046198136007)
                || (sl > 89.0013457619861 && sl < 89.0445839193854)
                || (sl > 74.000266017334 && sl < 74.0402776596676)
                || (sl > 59.0034474952262 && sl < 59.034047233342)
                || (sl > 44.0005194120248 && sl < 44.026037863635)
                || (sl > 29.0003734693891 && sl < 29.0216673913857)
                || (sl > 14.0025458727477 && sl < 14.015670561109)      // && Math.Round(sl, 10) != 14.0151613926)
                || (sl > 359.002244547314 && sl < 359.005330829566)
                || Math.Round(sl, 11) == 209.00033593538) { }   // ? 209.000335935378 ?
            else sl += 1.0;

            // 24 / 360 == 1 / 15
            int i = (int)Math.Floor(sl / 15);
            return minorSolarTerms[i % 24];
        }

        /// <summary>
        /// Returns the lunar year's name as a pairs of selestial stems and terrestrial branches.
        /// </summary>
        /// <param name="year">The lunar year to spelling.</param>
        /// <returns>The lunar year's name as a pairs of selestial stems and terrestrial branches.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Year is outside the range supported
        /// by this calendar.</exception>
        public static string GetYearName(int year)
        {
            CheckYearRange(year);

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}",
                selestialStems[(year + 6) % 10],
                terrestrialBranches[(year + 8) % 12]);
        }

        /// <summary>
        /// Returns the lunar day's name as a pairs of selestial stems and terrestrial branches.
        /// </summary>
        /// <param name="dt">The solar date.</param>
        /// <exception cref="ArgumentOutOfRangeException">Date is outside the range supported
        /// by this calendar.</exception>
        public static string GetDayName(DateTime dt)
        {
            CheckTicksRange(dt.Ticks);
            int jdn = JulianDayNumber(dt);

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}",
                selestialStems[(jdn + 9) % 10],
                terrestrialBranches[(jdn + 1) % 12]);
        }

        /// <summary>
        /// Returns the lunar month's name as a pairs of selestial stems and terrestrial branches.
        /// </summary>
        /// <param name="year">The lunar year.</param>
        /// <param name="month">The lunar month (from 1 to 13).</param>
        /// <exception cref="ArgumentOutOfRangeException">Date is outside the range supported
        /// by this calendar.</exception>
        public static string GetMonthName(int year, int month)
        {
            int leapMonth = GetLunarLeapMonth(year);
            CheckMonthRange(month, leapMonth);

            int i = (leapMonth > 0 && month >= leapMonth) ? -1 : 0;

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}{2}",
                selestialStems[(year * 12 + month + i + 3) % 10],
                terrestrialBranches[(month + i + 1) % 12],
                ((leapMonth > 0 && month == leapMonth) ? " (nhuận)" : string.Empty));
        }

        /// <summary>
        /// Returns the lunar month's speech name.
        /// </summary>
        /// <param name="year">The lunar year.</param>
        /// <param name="month">The lunar month (from 1 to 13).</param>
        /// <exception cref="ArgumentOutOfRangeException">Date is outside the range supported
        /// by this calendar.</exception>
        public static string GetMonthSpeechName(int year, int month)
        {
            int leapMonth = GetLunarLeapMonth(year);
            CheckMonthRange(month, leapMonth);

            int i = (leapMonth > 0 && month >= leapMonth) ? -1 : 0;

            return string.Format(
                CultureInfo.InvariantCulture,
                "Tháng {0}{1}",
                monthSpeechNames[month + i - 1],
                ((leapMonth > 0 && month == leapMonth) ? " (nhuận)" : string.Empty));
        }

        /// <summary>
        /// Returns a comma-separated string as a list of propitious hours in a day.
        /// </summary>
        /// <param name="dt">A gregorian <see cref="DateTime"/>.</param>
        /// <param name="withHours">Whether or not to return hours range for each propitious hours.</param>
        /// <returns>A comma-separated string.</returns>
        public static string GetPropitiousHour(DateTime dt, bool withHours = true)
        {
            int jdn = JulianDayNumber(dt);
            int chiOfDay = (jdn + 1) % 12;

            //// Same values for Ty' (1) and Ngo. (6), for Suu and Mui etc.
            string gioHD = propitiousHour[chiOfDay % 6];

            StringBuilder sb = new StringBuilder();
            string format = (withHours) ? "{0} ({1}-{2}), " : "{0}, ";
            for (int i = 0; i < 12; i++)
            {
                if (gioHD[i] != '0')
                {
                    sb.AppendFormat(
                        CultureInfo.CurrentCulture,
                        format,
                        terrestrialBranches[i],
                        (i * 2 + 23) % 24,
                        (i * 2 + 1) % 24);

                    ////if (++count % 3 == 0) ret.AppendLine();
                }
            }

            if (sb.Length > 0 && sb.ToString().EndsWith(", ", StringComparison.Ordinal))
            {
                sb.Length -= 2;    // Trims last commas
            }

            return sb.ToString();
        }

        /// <summary>
        /// 'Can' of the hour 'Chinh Ty' (00:00) of the day in proportion to this JDN.
        /// </summary>
        public static string GetHourZeroName(DateTime time)
        {
            int jdn = JulianDayNumber(time);

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}",
                selestialStems[(jdn - 1) * 2 % 10],
                terrestrialBranches[0]);
        }

        #endregion
    }
}
