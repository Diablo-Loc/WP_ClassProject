using System;
using System.Collections.Concurrent;
using System.Data;
using System.IO;

namespace ClassProject.Business.Services
{
    public class ConnectionTraceInfo
    {
        public string ConnectionId { get; set; }
        public DateTime OpenedAt { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public bool IsAlerted { get; set; }
    }

    public static class ConnectionMonitor
    {
        // Bộ nhớ đệm lưu các kết nối đang sống
        public static readonly ConcurrentDictionary<string, ConnectionTraceInfo> ActiveConnections = new ConcurrentDictionary<string, ConnectionTraceInfo>();

        // Bảng dữ liệu chứa danh sách các lỗi phục vụ hiển thị lên GridView của Admin
        public static readonly DataTable LeakHistoryTable = new DataTable();

        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connection_ai_log.txt");
        private static readonly object LogLock = new object();

        // Sự kiện thông báo cho Form Monitor biết để cập nhật giao diện lập tức
        public static event Action OnDataChanged;

        static ConnectionMonitor()
        {
            // Định hình khung dữ liệu hiển thị cho Admin
            LeakHistoryTable.Columns.Add("Time", typeof(string));
            LeakHistoryTable.Columns.Add("ConnID", typeof(string));
            LeakHistoryTable.Columns.Add("File", typeof(string));
            LeakHistoryTable.Columns.Add("Line", typeof(int));
            LeakHistoryTable.Columns.Add("Duration", typeof(string));
            LeakHistoryTable.Columns.Add("Status", typeof(string));
        }

        public static void RegisterOpen(string connId, string filePath, int lineNumber)
        {
            var trace = new ConnectionTraceInfo
            {
                ConnectionId = connId,
                OpenedAt = DateTime.Now,
                FilePath = Path.GetFileName(filePath),
                LineNumber = lineNumber
            };
            ActiveConnections.TryAdd(connId, trace);

            WriteLog($"[OPEN] 🟢 Conn: {connId} tại {trace.FilePath} (Dòng {lineNumber})");
            OnDataChanged?.Invoke(); // Cập nhật số lượng kết nối đang mở lên Dashboard
        }

        public static void RegisterClose(string connId)
        {
            if (ActiveConnections.TryRemove(connId, out var trace))
            {
                double duration = (DateTime.Now - trace.OpenedAt).TotalSeconds;
                WriteLog($"[CLOSE] 🔴 Conn: {connId} | Thời gian mở: {duration:F2}s");
                OnDataChanged?.Invoke();
            }
        }

        /// <summary>
        /// Hàm này sẽ được Form Monitor gọi liên tục bằng Timer (ví dụ mỗi 1 giây) để quét lỗi
        /// </summary>
        public static void CheckForLeaks()
        {
            DateTime now = DateTime.Now;
            double leakThresholdSeconds = 3.0; // Treo quá 3 giây coi như rò rỉ (Dễ demo)
            bool hashNewLeak = false;

            foreach (var item in ActiveConnections.Values)
            {
                double activeTime = (now - item.OpenedAt).TotalSeconds;
                if (activeTime > leakThresholdSeconds && !item.IsAlerted)
                {
                    item.IsAlerted = true;
                    hashNewLeak = true;

                    // Thêm bản ghi rò rỉ vào bảng lịch sử của Admin
                    DataRow row = LeakHistoryTable.NewRow();
                    row["Time"] = now.ToString("HH:mm:ss");
                    row["ConnID"] = item.ConnectionId;
                    row["File"] = item.FilePath;
                    row["Line"] = item.LineNumber;
                    row["Duration"] = $"{activeTime:F1} giây";
                    row["Status"] = "⚠️ ĐANG RÒ RỈ";
                    LeakHistoryTable.Rows.InsertAt(row, 0); // Đẩy lỗi mới nhất lên đầu bảng

                    WriteLog($"[LEAK ALERT] ⚠️ Treo kết nối tại {item.FilePath} (Dòng {item.LineNumber})");
                }
            }

            if (hashNewLeak)
            {
                OnDataChanged?.Invoke(); // Kích hoạt làm mới giao diện Monitor
            }
        }

        private static void WriteLog(string message)
        {
            lock (LogLock)
            {
                try
                {
                    File.AppendAllText(LogFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
                }
                catch { }
            }
        }
    }
}