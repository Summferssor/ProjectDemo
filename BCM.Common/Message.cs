using System;
using System.Collections.Generic;
using System.Text;

namespace BCM.Common
{
    public class Message
    {
        /// <summary>
        /// 消息码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 消息对象
        /// </summary>
        public Dictionary<string, object> Data = new Dictionary<string, object>();

        public static Message Ok()
        {
            Message result = new Message
            {
                Code = 200,
                Msg = "处理成功！"
            };
            return result;
        }

        public static Message Fail()
        {
            Message result = new Message
            {
                Code = 400,
                Msg = "处理失败！"
            };
            return result;
        }

        public static Message VerifyFail()
        {
            Message result = new Message
            {
                Code = 401,
                Msg = "验证失败！"
            };
            return result;
        }
    
        public static Message ServerError()
        {
            Message result = new Message
            {
                Code = 500,
                Msg = "服务器内部错误！"
            };
            return result;
        }

        public static Message NotFound()
        {
            Message result = new Message
            {
                Code = 404,
                Msg = "未找到资源！"
            };
            return result;
        }

        public Message Add(string key, object value)
        {
            GetData().Add(key, value);
            return this;
        }

        public Dictionary<string, object> GetData()
        {
            return Data;
        }

        public void SetData(Dictionary<string, object> data)
        {
            Data = data;
        }
    }
}
