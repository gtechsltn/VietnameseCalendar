using System;
using System.Globalization;
using Xunit;

namespace VietnameseCalendarUnitTests
{
    public class CalendarTests
    {
        /// <summary>
        /// Convert between GregorianCalendar and ChineseLunisolarCalendar
        /// https://stackoverflow.com/questions/19075759/convert-between-calendars
        /// </summary>
        [Fact]
        public void Convert_Between_GregorianCalendar_And_ChineseLunisolarCalendar()
        {
            var gregorianCalendar = new GregorianCalendar();
            var chineseCalendar = new ChineseLunisolarCalendar();

            var gregorianDate = new DateTime(1983, 6, 25, gregorianCalendar);

            var y = chineseCalendar.GetYear(gregorianDate);
            var m = chineseCalendar.GetMonth(gregorianDate);
            var d = chineseCalendar.GetDayOfMonth(gregorianDate);

            Assert.Equal(1983, y);
            Assert.Equal(5, m);
            Assert.Equal(15, d);
        }

        /// <summary>
        /// Convert between ChineseLunisolarCalendar and GregorianCalendar
        /// https://stackoverflow.com/questions/19075759/convert-between-calendars
        /// </summary>
        [Fact]
        public void Convert_Between_ChineseLunisolarCalendar_And_GregorianCalendar()
        {
            var chineseCalendar = new ChineseLunisolarCalendar();
            var cal = new GregorianCalendar();

            var chineseDate = new DateTime(2024, 8, 22, chineseCalendar);
            var y = cal.GetYear(chineseDate);
            var m = cal.GetMonth(chineseDate);
            var d = cal.GetDayOfMonth(chineseDate);

            Assert.Equal(2024, y);
            Assert.Equal(9, m);
            Assert.Equal(24, d);
        }

        /// <summary>
        /// System.Globalization.ChineseLunisolarCalendar
        /// https://github.com/dotnet/dotnet-api-docs/tree/main/xml/System.Globalization
        /// + ChineseLunisolarCalendar.xml
        /// + Exception: JapaneseLunisolarCalendar.xml
        /// + KoreanLunisolarCalendar.xml
        /// + Exception: TaiwanLunisolarCalendar.xml
        /// </summary>
        [Fact]
        public void WhenDateTimeHasCalendar_ThenTheInternalRepresentationIsInChineseLunisolarCalendar()
        {
            var chineseCalendar = new ChineseLunisolarCalendar();
            var dateTime = new DateTime(2024, 8, 22, chineseCalendar);

            Assert.Equal(2024, dateTime.Year);
            Assert.Equal(9, dateTime.Month);
            Assert.Equal(24, dateTime.Day);
        }

        [Fact]
        public void WhenDateTimeHasCalendar_ThenTheInternalRepresentationIsInKoreanLunisolarCalendar()
        {
            var calendar = new KoreanLunisolarCalendar();
            var dateTime = new DateTime(2024, 8, 22, calendar);

            Assert.Equal(2024, dateTime.Year);
            Assert.Equal(9, dateTime.Month);
            Assert.Equal(24, dateTime.Day);
        }

        [Fact]
        public void WhenUsingCalendar_ThenGetsDaysInMonth()
        {
            var calendar = new GregorianCalendar();
            var daysInJanuary = calendar.GetDaysInMonth(2023, 1);

            Assert.Equal(31, daysInJanuary);
        }

        [Fact]
        public void WhenDateTimeHasCalendar_ThenTheInternalRepresentationIsInGregorianCalendar()
        {
            var calendar = new HijriCalendar();
            var dateTime = new DateTime(1444, 4, 2, calendar);

            Assert.Equal(2022, dateTime.Year);
            Assert.Equal(10, dateTime.Month);
            Assert.Equal(27, dateTime.Day);
        }

        [Fact]
        public void WhenUsingCultureInfo_ThenGetOptionalCalendars()
        {
            var cultureInfo = new CultureInfo("ar-SA");
            var optionalCalendars = cultureInfo.OptionalCalendars;

            Assert.Equal(3, optionalCalendars.Length);
            Assert.Equal("UmAlQuraCalendar", optionalCalendars[0].GetType().Name);
            Assert.Equal("GregorianCalendar", optionalCalendars[1].GetType().Name);
            Assert.Equal("HijriCalendar", optionalCalendars[2].GetType().Name);
        }

        [Fact]
        public void WhenUsingCultureInfo_ThenPrintCorrectFormat()
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            var date = new DateTime(2023, 06, 18, 14, 00, 00);

            var formattedDate = date.ToString("f");

            Assert.Equal("dimanche 18 juin 2023 14:00", formattedDate);
        }

        [Fact]
        public void WhenUsingSaudiCultureInfoWithFrenchFormatProvider_ThenPrintFrenchFormat()
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            var date = new DateTime(2023, 06, 18, 14, 00, 00);

            var formattedDate = date.ToString("f", new CultureInfo("fr-FR"));

            Assert.Equal("dimanche 18 juin 2023 14:00", formattedDate);
        }

        [Fact]
        public void WhenUsingCultureInfo_ThenGetsCalendar()
        {
            var usCulture = new CultureInfo("en-US");
            var usCalendar = usCulture.Calendar;

            var daysInMonth = usCalendar.GetDaysInMonth(2023, 3);

            Assert.Equal(31, daysInMonth);
        }

        [Fact]
        public void WhenUsingCultureInfo_CanCompareDates()
        {
            var usCulture = new CultureInfo("en-US");
            var saudiCulture = new CultureInfo("ar-SA");

            var usDate = new DateTime(2023, 3, 21, usCulture.Calendar);
            var saudiDate = new DateTime(1444, 8, 10, saudiCulture.Calendar);

            Assert.True(usDate > saudiDate);
        }
    }
}