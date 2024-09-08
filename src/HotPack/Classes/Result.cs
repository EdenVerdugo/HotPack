﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.Classes
{
    public class Result
    {
        public bool Value { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        private string _InfoMessage { get; set; } = string.Empty;
        public int Code { get; set; }

        public Result()
        {

        }

        public Result(bool value, string msg)
        {
            this.Value = value;
            this.Message = msg;
        }

        public Result(bool value, string msg, object data)
        {
            this.Value = value;
            this.Message = msg;
            this.Data = data;
        }

        public Result(Exception ex)
        {
            this.Value = false;
            this.Message = ex.Message;
        }

        public static Result Create(bool value, string msg)
        {
            return new Result(value, msg);
        }

        public static Result Create(bool value, string msg, object data)
        {
            return new Result(value, msg, data);
        }

        public static Result Create(Exception ex)
        {
            return new Result(ex);
        }

        public string InfoMessage()
        {
            return _InfoMessage;
        }

        public string InfoMessage(string infoMessageValue)
        {
            _InfoMessage = infoMessageValue;

            return InfoMessage();
        }
    }

    public class Result<T> 
    {
        public bool Value { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int Code { get; set; }

        public Result()
        {

        }

        public Result(bool value, string msg)
        {
            this.Value = value;
            this.Message = msg;
        }

        public Result(bool value, string msg, T data)
        {
            this.Value = value;
            this.Message = msg;
            this.Data = data;
        }

        public Result(Exception ex)
        {
            this.Value = false;
            this.Message = ex.Message;
        }

        public static Result Create(bool value, string msg)
        {
            return new Result(value, msg);
        }

        public static Result Create(bool value, string msg, T data)
        {
            return new Result(value, msg, data!);
        }

        public static Result Create(Exception ex)
        {
            return new Result(ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="M">Tipo de valor devuelto en el Data</typeparam>
        /// <param name="func">Funcion de mapeo con AutoMapper</param>
        /// <returns></returns>
        public Result<M> Map<M>(Func<T?, M> func)
        {
            var result = new Result<M>();
            result.Code = this.Code;
            result.Message = this.Message;
            result.Value = this.Value;

            result.Data = func(this.Data);

            return result;
        }
    }    

    public class ResultList<T> : Result<List<T>>
    {
        public ResultList() 
        {

        }
    }
}
