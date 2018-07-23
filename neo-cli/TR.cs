using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DbgViewTR
{
#if DEBUG

    class DbgData
    {
        public string file_name;
        public string method_name;
        public int line_number;
        public int column_number;
        public int thread_id;
        public DbgData(StackFrame sf)
        {
            file_name = sf.GetFileName();
            int slash_location = file_name.LastIndexOf('\\');
            if (slash_location > 0)
            {
                file_name = file_name.Substring(slash_location + 1);
            }
            method_name = sf.GetMethod().Name;
            line_number = sf.GetFileLineNumber();
            column_number = sf.GetFileColumnNumber();
            thread_id = Thread.CurrentThread.ManagedThreadId;
        }
    }
#endif

    class TR
    {
#if DEBUG
        const string project_key = "NEO-CLI-DBG";
        private static ThreadLocal<int> indent = new ThreadLocal<int>(() => 1);
#endif

        public static void enter()
        {
#if DEBUG
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            DbgData dd = new DbgData(sf);
            log(dd, ">");
#endif
        }

        public static void exit()
        {
#if DEBUG
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            DbgData dd = new DbgData(sf);
            log(dd, "<");
#endif
        }

        public static T exit<T>(T result)
        {
#if DEBUG
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            DbgData dd = new DbgData(sf);
            log(dd, "return {0}", result.ToString());
            log(dd, "<");
#endif
            return result;
        }

        public static void log(string format, params object[] args)
        {
#if DEBUG
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(1);
            DbgData dd = new DbgData(sf);
            log(dd, format, args);
#endif
        }

#if DEBUG
        private static void log(DbgData dd, string format, params object[] args)
        {
            if (format == "<")
            {
                indent.Value -= 2;
            }
            string indentStr = "".PadLeft(indent.Value);
            string dbgStr = String.Format("[{0}][{1}]{2}{3}({4}){5}()", project_key, dd.thread_id, indentStr, dd.file_name, dd.line_number, dd.method_name);
            string logStr = String.Format(format, args);
            string finalStr = String.Format("{0} : {1}", dbgStr, logStr);
            //Console.WriteLine(finalStr);
            Debug.WriteLine(finalStr);
            if (format == ">")
            {
                indent.Value += 2;
            }
        }
#endif
    }
}
