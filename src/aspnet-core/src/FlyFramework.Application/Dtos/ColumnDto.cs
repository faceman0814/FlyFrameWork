using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Dtos
{
    public class ColumnDto
    {
        /// <summary>
        /// 显示的标题
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string prop { get; set; }
        /// <summary>
        /// 指定插槽
        /// </summary>
        public string slot { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string render { get; set; }

        /// <summary>
        /// 日期格式化,默认值为 'yyyy-MM-dd HH:mm:ss'
        /// </summary>
        public string dateFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 是否显示超出省略号，默认值为 true
        /// </summary>
        public bool showOverflowTooltip { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public string linkType { get; set; }

        ///// <summary>
        /////  对应列的类型，
        /////  如果设置了 `selection` 则显示多选框；
        /////  如果设置了 `index` 则显示该行的索引（从 `1` 开始计算）；
        /////  如果设置了 `expand` 则显示为一个可展开的按钮
        ///// </summary>
        //public string type { get; set; }

        ///// <summary>
        ///// 如果设置了 `type=index`，可以通过传递 `index` 属性来自定义索引
        ///// </summary>
        //public int index { get; set; }

        ///// <summary>
        ///// `column` 的 `key`， 如果需要使用 `filter-change` 事件，则需要此属性标识是哪个 `column` 的筛选条件
        ///// </summary>
        //public string columnKey { get; set; }

        /// <summary>
        /// 对应列的宽度
        /// </summary>
        public string width { get; set; }

        ///// <summary>
        ///// 对应列的最小宽度
        ///// </summary>
        //public string minWidth { get; set; }

        /////// <summary>
        /////// 列是否固定在左侧或者右侧。`true` 表示固定在左侧
        /////// </summary>
        ////public string Fixed { get; set; } = "true";

        ///// <summary>
        ///// 对应列是否可以排序
        ///// 如果设置为 `'custom'`，则代表用户希望远程排序，需要监听 `Table` 的 `sort-change `事件
        ///// 默认值为 `false` 
        ///// </summary>
        //public string sortable { get; set; } = "custom";

        ///// <summary>
        ///// 对应列是否可以通过拖动改变宽度（需要在 `el-table` 上设置 `border` 属性为真），默认值为 `true`
        ///// </summary>
        //public bool resizable { get; set; } = true;

        ///// <summary>
        ///// 列标题 `Label` 区域渲染使用的 `Function` 
        ///// </summary>
        //public string renderHeader { get; set; }

        ///// <summary>
        ///// 指定数据按照哪个属性进行排序，仅当 `sortable` 设置为 `true` 的时候有效。应该如同 `Array.sort` 那样返回一个 `Number`
        ///// </summary>
        //public int sortMethod { get; set; }

        ///// <summary>
        ///// 指定数据按照哪个属性进行排序，仅当 `sortable` 设置为 `true` 且没有设置 `sort-method` 的时候有效。
        ///// 如果 `sort-by` 为数组，则先按照第 `1` 个属性排序，
        ///// 如果第 `1` 个相等，再按照第 `2` 个排序，以此类推 
        ///// </summary>
        //public List<string> sortBy { get; set; }

        /// <summary>
        /// 数据在排序时所使用排序策略的轮转顺序，仅当 `sortable` 为 `true` 时有效。
        /// 需传入一个数组，随着用户点击表头，该列依次按照数组中元素的顺序进行排序，
        /// 默认值为 `['ascending', 'descending', null]` 
        /// </summary>
        public List<string> sortOrders { get; set; } = ["ascending", "descending", null];

        ///// <summary>
        ///// 用来格式化内容的函数，仅对当前列有效。
        ///// </summary>
        //public string formatter { get; set; }

        ///// <summary>
        /////  当内容过长被隐藏时显示 `tooltip`，默认值为 `false`
        ///// </summary>
        //public bool showOverflowTooltip { get; set; }

        /// <summary>
        /// 对齐方式，可选值为 `left`、`center`、`right`
        /// </summary>
        public string align { get; set; } = "left";

        ///// <summary>
        ///// 表头对齐方式，若不设置该项，则使用表格的对齐方式
        ///// </summary>
        //public string headerAlign { get; set; } = "left";

        ///// <summary>
        ///// 列的 `className`
        ///// </summary>
        //public string className { get; set; }

        ///// <summary>
        ///// 当前列标题的自定义类名
        ///// </summary>
        //public string labelClassName { get; set; }

        ///// <summary>
        ///// 仅对 `type=selection` 的列有效，类型为 `Function`，`Function` 的返回值用来决定这一行的 `CheckBox` 是否可以勾选 
        ///// </summary>
        //public bool selectable { get; set; }

        ///// <summary>
        ///// 仅对 `type=selection` 的列有效，请注意，需指定 `row-key` 来让这个功能生效，默认值为 `false`
        ///// </summary>
        //public string reserveSelection { get; set; }
    }
}
