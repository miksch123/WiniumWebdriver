namespace OpenQA.Selenium.Winium
{
    #region using

    using System;
    using System.Reflection;

    using OpenQA.Selenium.Remote;

    #endregion

    public class WiniumDriverCommandExecutor : ICommandExecutor
    {
        #region Fields

        private ICommandExecutor internalExecutor;

        private WiniumDriverService service;

        #endregion

        #region Constructors and Destructors

        public WiniumDriverCommandExecutor(WiniumDriverService driverService, TimeSpan commandTimeout)
        {
            this.service = driverService;
            this.internalExecutor = CommandExecutorFactory.GetHttpCommandExecutor(driverService.ServiceUrl, commandTimeout);
        }

        #endregion

        #region Public Properties

        public CommandInfoRepository CommandInfoRepository
        {
            get
            {
                return this.internalExecutor.CommandInfoRepository;
            }
        }

        public void Dispose()
        {
            if(this.service.IsRunning is true)
            {
                int id = this.service.ProcessId;
                this.service.Dispose();
                while(id != null)
                {
                    System.Threading.Thread.Sleep(200);
                }
                
            }
        }

        #endregion

        #region Public Methods and Operators

        public Response Execute(Command commandToExecute)
        {
            if (commandToExecute == null)
            {
                throw new ArgumentNullException("commandToExecute", "Command may not be null");
            }

            if (commandToExecute.Name == DriverCommand.NewSession)
            {
                this.service.Start();
            }

            try
            {
                return this.internalExecutor.Execute(commandToExecute);
            }
            finally
            {
                if (commandToExecute.Name == DriverCommand.Quit)
                {
                    this.service.Dispose();
                }
            }
        }

        #endregion
    }
}
