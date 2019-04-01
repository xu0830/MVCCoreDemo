using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Services.Stations.Dtos
{
    /// <summary>
    /// 乘客信息
    /// </summary>
    public class PassengerData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Notify_for_gat { get; set; }

        /// <summary>
        /// 是否存在
        /// </summary>
        public bool IsExist { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ExMsg { get; set; }

        public List<int> Two_isOpenClick { get; set; }

        public List<int> Other_isOpenClick { get; set; }

        /// <summary>
        /// 常用乘客
        /// </summary>
        public List<PassengerDto> Normal_passengers { get; set; }

        /// <summary>
        /// 受让人
        /// </summary>
        public List<PassengerDto> Dj_passengers { get; set; }
    }

    /// <summary>
    /// 乘客Dto
    /// </summary>
    public class PassengerDto
    {
        public int Code { get; set; }

        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string Passenger_name { get; set; }

        /// <summary>
        /// 性别编码    M -> 男
        /// </summary>
        public string Sex_code { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex_name { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Born_data { get; set; }

        /// <summary>
        /// 国家代码
        /// </summary>
        public string Country_code { get; set; }

        /// <summary>
        /// 乘客身份证类型编码
        /// </summary>
        public int Passenger_id_type_code { get; set; }

        /// <summary>
        /// 乘客身份证类型
        /// </summary>
        public string Passenger_id_type_name { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Passenger_id_no { get; set; }

        /// <summary>
        /// 乘客类型
        /// </summary>
        public int Passenger_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Passenger_flag { get; set; }

        /// <summary>
        /// 乘客类型名称  成人/学生
        /// </summary>
        public string Passenger_type_name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Phone_no { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 姓名首字母
        /// </summary>
        public string First_letter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Total_times { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gat_born_date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gat_valid_date_start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gat_valid_date_end { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gat_version { get; set; }

    }
}
