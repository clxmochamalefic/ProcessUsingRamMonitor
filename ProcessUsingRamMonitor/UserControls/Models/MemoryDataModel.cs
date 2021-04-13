using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessUsingRamMonitor.UserControls.Models
{
    /// <summary>
    /// メモリタイプ
    /// </summary>
    public enum MemoryType
    {
        /// <summary>
        /// 物理確保メモリ
        /// </summary>
        WorkingSet,
        /// <summary>
        /// 物理使用メモリ
        /// </summary>
        PeakWorkingSet,
        /// <summary>
        /// ページング確保メモリ
        /// </summary>
        Paged,
        /// <summary>
        /// ページング使用メモリ
        /// </summary>
        PeakPaged,
        /// <summary>
        /// 仮想確保メモリ
        /// </summary>
        Virtual,
        /// <summary>
        /// 仮想使用メモリ
        /// </summary>
        PeakVirtual,
        /// <summary>
        /// プライベートメモリ
        /// </summary>
        Private
    }
    public class MemoryDataModel
    {
        /// <summary>
        /// 計測日時
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// メモリ量
        /// </summary>
        public long Value { get; set; }
        /// <summary>
        /// メモリタイプ
        /// </summary>
        public MemoryType MemoryType { get; set; }
    }

    public record RecordRamMemoryDataModel
    {
        /// <summary>
        /// 記録日時
        /// </summary>
        public DateTime RecordTime { get; set; }
        /// <summary>
        /// 物理確保メモリ
        /// </summary>
        public long WorkingSet { get; set; }
        /// <summary>
        /// 物理使用メモリ
        /// </summary>
        public long PeakWorkingSet { get; set; }
        /// <summary>
        /// ページング確保メモリ
        /// </summary>
        public long Paged { get; set; }
        /// <summary>
        /// ページング使用メモリ
        /// </summary>
        public long PeakPaged { get; set; }
        /// <summary>
        /// 仮想確保メモリ
        /// </summary>
        public long Virtual { get; set; }
        /// <summary>
        /// 仮想使用メモリ
        /// </summary>
        public long PeakVirtual { get; set; }
        /// <summary>
        /// プライベートメモリ
        /// </summary>
        public long Private { get; set; }
    }
}
