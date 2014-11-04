using System;
using System.Threading;
using SharpExtensionsUtil.Logging;

namespace SharpExtensionsUtil.ThreadSafe
{
    public abstract class ManagerBase : IDisposable
    {
        protected virtual string ModuleName
        {
            get { return string.Empty; }
        }

        protected bool useLogNoSql = true;

        Thread ThreadWork;
        protected readonly ManualResetEvent EventStop = new ManualResetEvent(false);
        private const int timeoutIntervalMils = 15000;
        protected int timeoutIntervalsWorkThreadMils = 15000;

        protected void StartThread()
        {
            ThreadWork = new Thread(Work) { Name = ModuleName };
            ThreadWork.Start();
            Logger.Info("Служба запущена");
        }

        void Work()
        {
            do
            {
                try
                {
                    CustomThreadWork();
                }

                catch (Exception ex)
                {
                    Logger.Error("Work: ", ex);
                    CustomHandlerWorkException(ex);
                }
            } while (EventStop.WaitOne(timeoutIntervalsWorkThreadMils) == false);
            PostEventThreadAborted();
        }

        public void Dispose()
        {
            try
            {
                Logger.Info("Служба остановлена");
                CustomDispose();
                EventStop.Set();

                if (ThreadWork.Join(timeoutIntervalMils)) return;

                Logger.Info("Вышло время ожидания остановки потока, принудительное завершение...");

                ThreadWork.Abort();
            }
            catch (Exception ex)
            {
                Logger.Error("Dispose: ", ex);
            }
        }

        /// <summary>
        /// Освобождение ресурсов наследников
        /// </summary>
        protected virtual void CustomDispose() { }

        /// <summary>
        /// Метод прокрутки задач внутри потока, перегружается наследниками для реализации собственной функциональности
        /// </summary>
        protected virtual void CustomThreadWork() { }

        /// <summary>
        /// обработчик исключений внутри потока ThreadWork
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void CustomHandlerWorkException(Exception ex) { }

        /// <summary>
        /// метод позволяет указать дополнительные действия после завершения рабочего потока
        /// </summary>
        protected virtual void PostEventThreadAborted() { }
    }
}