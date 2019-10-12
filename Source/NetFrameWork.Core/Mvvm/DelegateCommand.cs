using System;
using System.ComponentModel;
using System.Windows.Input;

namespace NetFrameWork.Core.Mvvm
{
    /// <summary>
    /// 委托命令
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// 命令执行前事件
        /// </summary>
        public event EventHandler<CancelEventArgs> Executing;

        /// <summary>
        /// 命令执行后事件
        /// </summary>
        public event EventHandler Executed;

        private bool _canExecuteCache;

        /// <inheritdoc />
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="executeActionFunc">执行函数</param>
        public DelegateCommand(Action<object> executeActionFunc) : this(executeActionFunc, null) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="executeActionFunc">执行函数</param>
        /// <param name="canExecuteFunc">可执行函数</param>
        public DelegateCommand(Action<object> executeActionFunc, Func<object, bool> canExecuteFunc)
        {
            ExecuteActionFunc = executeActionFunc;
            CanExecuteFunc = canExecuteFunc;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        public Action<object> ExecuteActionFunc { get; protected set; }

        /// <summary>
        /// 可执行函数
        /// </summary>
        public Func<object, bool> CanExecuteFunc { get; protected set; }

        #region ICommand Members

        /// <summary>
        /// 允许执行变化事件
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 命令是否可执行
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns>是否可执行</returns>
        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunc == null)
            {
                return true;
            }
            var bResult = CanExecuteFunc(parameter);
            if (bResult == _canExecuteCache)
            {
                return bResult;
            }
            _canExecuteCache = bResult;
            CanExecuteChanged?.Invoke(parameter, EventArgs.Empty);
            return bResult;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">参数</param>
        public void Execute(object parameter)
        {
            var e = new CancelEventArgs(false);
            Executing?.Invoke(parameter, e);
            if (e.Cancel)
            {
                return;
            }

            ExecuteActionFunc(parameter);
            Executed?.Invoke(parameter, e);
        }

        #endregion
    }
}
