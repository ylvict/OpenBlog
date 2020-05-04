using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenBlog.Web.Models
{
    public class FormData
    {
        public string Note { get; set; }
    }

    /// <summary>
    /// 这里的中文是不建议的，后面需要重构
    /// </summary>
    public enum RemeberOptions
    {
        [Description("一天")]
        OneDay = 1,
        [Description("两天")]
        TwoDays = 2,
        [Description("三天")]
        ThreeDays = 3,
        [Description("一周")]
        OneWeek = 7,
        [Description("两周")]
        TwoWeeks = 14,
        [Description("一个月")]
        OneMonth = 30
    }


}
