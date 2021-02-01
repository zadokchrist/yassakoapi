﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using log4net;

namespace YakakoApi
{
    public class SoapLoggerExtension : SoapExtension
    {
        private static ILog Logger
        {
            get { return _Logger ?? (_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)); }
        }
        private static ILog _Logger;

        private Stream _OldStream;
        private Stream _NewStream;

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {

        }

        public override Stream ChainStream(Stream stream)
        {
            _OldStream = stream;
            _NewStream = new MemoryStream();
            return _NewStream;
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    Log(message, "AfterSerialize");
                    CopyStream(_NewStream, _OldStream);
                    _NewStream.Position = 0;
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    CopyStream(_OldStream, _NewStream);
                    Log(message, "BeforeDeserialize");
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
            }
        }

        public void Log(SoapMessage message, string stage)
        {
            _NewStream.Position = 0;
            Logger.Debug(stage);
            var reader = new StreamReader(_NewStream);
            string requestXml = reader.ReadToEnd();
            _NewStream.Position = 0;
            if (!string.IsNullOrWhiteSpace(requestXml))
            {
                string Title = Environment.NewLine + "-----" + stage + " at " + DateTime.Now + Environment.NewLine;
                string whatToLog = Title + GetUserFriendlyXML(requestXml) + Environment.NewLine;
                Logger.Debug(whatToLog);
            }

                
        }

        public String GetUserFriendlyXML(String XML)
        {
            String Result = XML;

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(XML);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException)
            {
            }
            finally
            {
                mStream.Close();
                writer.Close();
            }

            return Result;
        }

        public void ReverseIncomingStream()
        {
            ReverseStream(_NewStream);
        }

        public void ReverseOutgoingStream()
        {
            ReverseStream(_NewStream);
        }

        public void ReverseStream(Stream stream)
        {
            TextReader tr = new StreamReader(stream);
            string str = tr.ReadToEnd();
            char[] data = str.ToCharArray();
            Array.Reverse(data);
            string strReversed = new string(data);

            TextWriter tw = new StreamWriter(stream);
            stream.Position = 0;
            tw.Write(strReversed);
            tw.Flush();
        }

        private void CopyStream(Stream fromStream, Stream toStream)
        {
            try
            {
                StreamReader sr = new StreamReader(fromStream);
                StreamWriter sw = new StreamWriter(toStream);
                sw.WriteLine(sr.ReadToEnd());
                sw.Flush();
            }
            catch (Exception ex)
            {
                string message = String.Format("CopyStream failed because: {0}", ex.Message);
                Logger.Error(message, ex);
            }
        }
    }
}