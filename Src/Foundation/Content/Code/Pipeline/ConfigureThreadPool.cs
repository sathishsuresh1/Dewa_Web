using DEWAXP.Foundation.Logger;
using Sitecore.Annotations;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Reflection;
using Sitecore.Xml;
using System.Text;
using System.Threading;
using System.Xml;

namespace DEWAXP.Foundation.Content.Pipeline
{
    /// <summary>
    ///  Configures thread limits for <see cref="ThreadPool"/>.
    /// <para>Thread pool limits should be configured in machine config, however an access to machine config is not provided in cloud, thereby API-based approach must be taken.</para>
    /// </summary>
    [UsedImplicitly]
    public class ConfigureThreadPool : IInitializable
    {
        #region fields

        /// <summary>
        /// The log to output running thread pool configuration in 'audit' level.
        /// </summary>
        //[NotNull]
        //protected readonly BaseLog Log;

        /// <summary>
        /// Gets or sets the maximum worker threads.
        /// </summary>
        /// <value>
        /// The maximum worker threads.
        /// </value>
        protected int MaxWorkerThreadsPerCore;

        /// <summary>
        /// Gets or sets the maximum completion port threads.
        /// </summary>
        /// <value>
        /// The maximum completion port threads.
        /// </value>
        protected int MaxCompletionPortThreadsPerCore;

        /// <summary>
        /// Gets or sets the minimum worker threads.
        /// </summary>
        /// <value>
        /// The minimum worker threads.
        /// </value>
        protected int MinWorkerThreadsPerCore;

        /// <summary>
        /// Gets or sets the minimum completion port threads.
        /// </summary>
        /// <value>
        /// The minimum completion port threads.
        /// </value>
        protected int MinCompletionPortThreadsPerCore;

        #endregion fields

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureThreadPool"/> class.
        /// </summary>
        /// <param name="log">The log to output running thread pool configuration in 'audit' level.</param>
        public ConfigureThreadPool()
        {
            //this.Log = log;
        }

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="IInitializable" /> needs calling code to assign properties even after <see cref="Initialize"/> calls.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="IInitializable" /> assigns the  properties; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AssignProperties
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the processor count.
        /// </summary>
        /// <value>
        /// The processor count.
        /// </value>
        public virtual int ProcessorCount
        {
            get
            {
                return System.Environment.ProcessorCount;
            }
        }

        /// <summary>
        /// Gets the maximum worker threads.
        /// </summary>
        /// <value>
        /// The maximum worker threads.
        /// </value>
        public int MaxWorkerThreads
        {
            get
            {
                return this.MaxWorkerThreadsPerCore * this.ProcessorCount;
            }
        }

        /// <summary>
        /// Gets the maximum completion port threads.
        /// </summary>
        /// <value>
        /// The maximum completion port threads.
        /// </value>
        public int MaxCompletionPortThreads
        {
            get
            {
                return this.MaxCompletionPortThreadsPerCore * this.ProcessorCount;
            }
        }

        /// <summary>
        /// Gets the minimum worker threads.
        /// </summary>
        /// <value>
        /// The minimum worker threads.
        /// </value>
        public int MinWorkerThreads
        {
            get
            {
                return this.MinWorkerThreadsPerCore * this.ProcessorCount;
            }
        }

        /// <summary>
        /// Gets the minimum completion port threads.
        /// </summary>
        /// <value>
        /// The minimum completion port threads.
        /// </value>
        public int MinCompletionPortThreads
        {
            get
            {
                return this.MinCompletionPortThreadsPerCore * this.ProcessorCount;
            }
        }

        #endregion Properties

        /// <summary>
        /// Configures thread limits for <see cref="ThreadPool"/> based on configuration values.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public virtual void Process(PipelineArgs args)
        {
            // this.LogThreadPoolStatistics(args, "Default thread pool size:");

            if (this.MaxWorkerThreads != 0 && this.MaxCompletionPortThreads != 0)
            {
                this.SetMaxThreadPoolLimit(this.MaxWorkerThreads, this.MaxCompletionPortThreads, args);
            }

            if (this.MinWorkerThreads != 0 && this.MinCompletionPortThreads != 0)
            {
                this.SetMinThreadPoolLimit(this.MinWorkerThreads, this.MinCompletionPortThreads, args);
            }

            this.LogThreadPoolStatistics(args, "New thread pool size:");
        }

        /// <summary>
        /// Logs the <see cref="ThreadPool"/> statistics.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="header">The header for a message to begin with.</param>
        public virtual void LogThreadPoolStatistics([NotNull] PipelineArgs args, [NotNull] string header)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(header, "header");

            int minWorkerThreads;
            int minCompletionPortThreads;
            int maxWorkerThreads;
            int maxCompletionPortThreads;

            this.GetThreadStatistics(out minWorkerThreads, out minCompletionPortThreads, out maxWorkerThreads, out maxCompletionPortThreads);

            var message = new StringBuilder()
              .AppendLine(header)
              .AppendLine("Processor count:" + this.ProcessorCount)
              .AppendLine("minWorkerThreadsPerCore count:" + minWorkerThreads)
              .AppendLine("minCompletionPortThreadsPerCore count:" + minCompletionPortThreads)
              .AppendLine("maxWorkerThreadsPerCore count:" + maxWorkerThreads)
              .AppendLine("maxCompletionPortThreadsPerCore count:" + maxCompletionPortThreads)
              .ToString();

            LogService.Debug(message);
        }

        /// <summary>
        /// Gets the the <see cref="ThreadPool"/> statistics.
        /// </summary>
        /// <param name="minWorkerThreads">The minimum worker threads.</param>
        /// <param name="minCompletionPortThreads">The minimum completion port threads.</param>
        /// <param name="maxWorkerThreads">The maximum worker threads.</param>
        /// <param name="maxCompletionPortThreads">The maximum completion port threads.</param>
        public virtual void GetThreadStatistics(out int minWorkerThreads, out int minCompletionPortThreads, out int maxWorkerThreads, out int maxCompletionPortThreads)
        {
            System.Threading.ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);

            System.Threading.ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);
        }

        /// <summary>
        /// Sets the minimum thread pool limit.
        /// </summary>
        /// <param name="minWorkerThreads">The minimum worker threads.</param>
        /// <param name="minCompletionPortThreads">The minimum completion port threads.</param>
        /// <param name="args">The arguments.</param>
        public virtual void SetMinThreadPoolLimit(int minWorkerThreads, int minCompletionPortThreads, PipelineArgs args)
        {
            System.Threading.ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads);
        }

        /// <summary>
        /// Sets the maximum thread pool limit.
        /// </summary>
        /// <param name="maxWorkerThreads">The maximum worker threads.</param>
        /// <param name="maxCompletionPortThreads">The maximum completion port threads.</param>
        /// <param name="args">The arguments.</param>
        public virtual void SetMaxThreadPoolLimit(int maxWorkerThreads, int maxCompletionPortThreads, PipelineArgs args)
        {
            System.Threading.ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);
        }

        /// <summary>
        /// Initializes the object from the specified config node.
        /// </summary>
        /// <param name="configNode">The config node.</param>
        public virtual void Initialize(XmlNode configNode)
        {
            if (configNode == null)
            {
                return;
            }

            var attribute = XmlUtil.FindChildNode("MaxWorkerThreadsPerCore", configNode, false);
            if (attribute != null)
            {
                int.TryParse(attribute.InnerText, out this.MaxWorkerThreadsPerCore);
            }

            attribute = XmlUtil.FindChildNode("MaxCompletionPortThreadsPerCore", configNode, false);
            if (attribute != null)
            {
                int.TryParse(attribute.InnerText, out this.MaxCompletionPortThreadsPerCore);
            }

            attribute = XmlUtil.FindChildNode("MinWorkerThreadsPerCore", configNode, false);
            if (attribute != null)
            {
                int.TryParse(attribute.InnerText, out this.MinWorkerThreadsPerCore);
            }

            attribute = XmlUtil.FindChildNode("MinCompletionPortThreadsPerCore", configNode, false);
            if (attribute != null)
            {
                int.TryParse(attribute.InnerText, out this.MinCompletionPortThreadsPerCore);
            }
        }
    }
}