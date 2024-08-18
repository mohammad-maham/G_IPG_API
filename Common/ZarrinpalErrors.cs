namespace G_IPG_API.Common
{
    public   class ZarrinpalErrors
    {
        public static string GetZarrinpalError(int key)
        {
            var ErrorList = new Dictionary<int, string>{
            { -9, "مبلغ پرداختی کمتر   از حد مجاز است" },
            { -10, "ای پی یا مرچنت كد پذیرنده صحیح نیست." },
            { -11, "مرچنت کد فعال نیست،." },
            {-12,"تلاش بیش از دفعات مجاز "},
            {-15 ,"درگاه پرداخت به حالت تعلیق در آمده است،"},
            {100 ,"عملیات موفق"},
            { -32,"مبلغ وارد شده از مبلغ کل تراکنش بیشتر است."},
            { -51,"پرداخت ناموفق"},
            { -52,"خطای غیر منتظره‌ای رخ داده است."},
            { -55,"تراکنش مورد نظر یافت نشد"},
            { 101," تراکنش وریفای شده است."},
        };


            if (ErrorList.TryGetValue(key, out string errorMessage))
                if (!string.IsNullOrEmpty(errorMessage))
                    return errorMessage;

            return "خطای ناشناخته اتفاق افتاده است لطفا دوباره تلاش نمایید";

        }

    }
}
