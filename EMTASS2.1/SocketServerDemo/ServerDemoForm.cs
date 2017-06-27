using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CSUST.Net;

namespace EMTASS_ServerDemo
{
    public partial class SocketServerDemo : Form
    {
        TSocketServerBase<TTestSession, TTestAccessDatabase> m_socketServer;

        public SocketServerDemo()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void SocketServerDemo_Load(object sender, EventArgs e)
        {
            cb_maxDatagramSize.SelectedIndex = 1;
        }

        private void SocketServerDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_socketServer != null)
            {
                m_socketServer.Dispose();  // �رշ���������
            }
        }

        private void AttachServerEvent()
        {
            m_socketServer.ServerStarted += this.SocketServer_Started;
            m_socketServer.ServerClosed += this.SocketServer_Stoped;
            m_socketServer.ServerListenPaused += this.SocketServer_Paused;
            m_socketServer.ServerListenResumed += this.SocketServer_Resumed;
            m_socketServer.ServerException += this.SocketServer_Exception;

            m_socketServer.SessionRejected += this.SocketServer_SessionRejected;
            m_socketServer.SessionConnected += this.SocketServer_SessionConnected;
            m_socketServer.SessionDisconnected += this.SocketServer_SessionDisconnected;
            m_socketServer.SessionReceiveException += this.SocketServer_SessionReceiveException;
            m_socketServer.SessionSendException += this.SocketServer_SessionSendException;

            m_socketServer.DatagramDelimiterError += this.SocketServer_DatagramDelimiterError;
            m_socketServer.DatagramOversizeError += this.SocketServer_DatagramOversizeError;
            m_socketServer.DatagramAccepted += this.SocketServer_DatagramReceived;
            m_socketServer.DatagramError += this.SocketServer_DatagramrError;
            m_socketServer.DatagramHandled += this.SocketServer_DatagramHandled;

            if (ck_UseDatabase.Checked)
            {
                m_socketServer.DatabaseOpenException += this.SocketServer_DatabaseOpenException;
                m_socketServer.DatabaseCloseException += this.SocketServer_DatabaseCloseException;
                m_socketServer.DatabaseException += this.SocketServer_DatabaseException;
            }

            m_socketServer.ShowDebugMessage += this.SocketServer_ShowDebugMessage;
        }

        private void bn_Start_Click(object sender, EventArgs e)
        {

            string connStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source = DemoAccessDatabase.mdb;";

            if (ck_UseDatabase.Checked)
            {
                m_socketServer = new TSocketServerBase<TTestSession, TTestAccessDatabase>(1024, 32 * 1024, 64 * 1024, connStr);
            }
            else
            {
                m_socketServer = new TSocketServerBase<TTestSession, TTestAccessDatabase>();
            }

            m_socketServer.MaxDatagramSize = 1024 * int.Parse(cb_maxDatagramSize.Text);

            this.AttachServerEvent();  // ���ӷ�����ȫ���¼�
            m_socketServer.Start();
        }

        private void bn_Stop_Click(object sender, EventArgs e)
        {
            if (m_socketServer != null)
            {
                m_socketServer.Stop();
                m_socketServer.Dispose();
            }
        }

        private void bn_Pause_Click(object sender, EventArgs e)
        {
            m_socketServer.PauseListen();
        }

        private void bn_Resume_Click(object sender, EventArgs e)
        {
            m_socketServer.ResumeListen();        
        }

        private void SocketServer_Started(object sender, EventArgs e)
        {
            this.AddInfo("Server started at: " + DateTime.Now.ToString());
        }

        private void SocketServer_Stoped(object sender, EventArgs e)
        {
            this.AddInfo("Server stoped at: " + DateTime.Now.ToString());
        }

        private void SocketServer_Paused(object sender, EventArgs e)
        {
            this.AddInfo("Server paused at: " + DateTime.Now.ToString());
        }

        private void SocketServer_Resumed(object sender, EventArgs e)
        {
            this.AddInfo("Server resumed at: " + DateTime.Now.ToString());
        }

        private void SocketServer_Exception(object sender, TExceptionEventArgs e)
        {
            this.tb_ServerExceptionCount.Text = m_socketServer.ServerExceptionCount.ToString();
            this.AddInfo("Server exception: " + e.ExceptionMessage);
        }

        private void SocketServer_SessionRejected(object sender, EventArgs e)
        {
            this.AddInfo("Session connect rejected");
        }

        private void SocketServer_SessionTimeout(object sender, TSessionEventArgs e)
        {
            this.AddInfo("Session timeout: ip " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_SessionConnected(object sender, TSessionEventArgs e)
        {
            this.tb_SessionCount.Text = m_socketServer.SessionCount.ToString();
            this.AddInfo("Session connected: ip " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_SessionDisconnected(object sender, TSessionEventArgs e)
        {
            this.tb_SessionCount.Text = m_socketServer.SessionCount.ToString();
            this.AddInfo("Session disconnected: ip " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_SessionReceiveException(object sender, TSessionEventArgs e)
        {
            this.tb_SessionCount.Text = m_socketServer.SessionCount.ToString();
            this.tb_ClientExceptionCount.Text = m_socketServer.SessionExceptionCount.ToString();
            this.AddInfo("Session receive exception: ip " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_SessionSendException(object sender, TSessionEventArgs e)
        {
            this.tb_SessionCount.Text = m_socketServer.SessionCount.ToString();
            this.tb_ClientExceptionCount.Text = m_socketServer.SessionExceptionCount.ToString();
            this.AddInfo("Session send exception: ip " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_SocketReceiveException(object sender, TSessionExceptionEventArgs e)
        {
            this.tb_SessionCount.Text = m_socketServer.SessionCount.ToString();
            this.tb_ClientExceptionCount.Text = m_socketServer.SessionExceptionCount.ToString();
            this.AddInfo("client socket receive exception: ip: " + e.SessionBaseInfo.IP + " exception message: " + e.ExceptionMessage);
        }

        private void SocketServer_SocketSendException(object sender, TSessionExceptionEventArgs e)
        {
            this.tb_SessionCount.Text = m_socketServer.SessionCount.ToString();
            this.tb_ClientExceptionCount.Text = m_socketServer.SessionExceptionCount.ToString();
            this.AddInfo("client socket send exception: ip: " + e.SessionBaseInfo.IP + " exception message: " + e.ExceptionMessage);
        }

        private void SocketServer_DatagramDelimiterError(object sender, TSessionEventArgs e)
        {
            this.tb_DatagramCount.Text = m_socketServer.ReceivedDatagramCount.ToString();
            this.tb_DatagramQueueCount.Text = m_socketServer.DatagramQueueLength.ToString();
            this.tb_ErrorDatagramCount.Text = m_socketServer.ErrorDatagramCount.ToString();

            this.AddInfo("datagram delimiter error. ip: " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_DatagramOversizeError(object sender, TSessionEventArgs e)
        {
            this.tb_DatagramCount.Text = m_socketServer.ReceivedDatagramCount.ToString();
            this.tb_DatagramQueueCount.Text = m_socketServer.DatagramQueueLength.ToString();
            this.tb_ErrorDatagramCount.Text = m_socketServer.ErrorDatagramCount.ToString();

            this.AddInfo("datagram oversize error. ip: " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_DatagramReceived(object sender, TSessionEventArgs e)
        {
            this.tb_DatagramCount.Text = m_socketServer.ReceivedDatagramCount.ToString();
            this.tb_DatagramQueueCount.Text = m_socketServer.DatagramQueueLength.ToString();
            this.AddInfo("datagram received. ip: " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_DatagramrError(object sender, TSessionEventArgs e)
        {
            this.tb_DatagramCount.Text = m_socketServer.ReceivedDatagramCount.ToString();
            this.tb_DatagramQueueCount.Text = m_socketServer.DatagramQueueLength.ToString();
            this.tb_ErrorDatagramCount.Text = m_socketServer.ErrorDatagramCount.ToString();

            this.AddInfo("datagram error. ip: " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_DatagramHandled(object sender, TSessionEventArgs e)
        {
            this.tb_DatagramCount.Text = m_socketServer.ReceivedDatagramCount.ToString();
            this.tb_DatagramQueueCount.Text = m_socketServer.DatagramQueueLength.ToString();
            this.AddInfo("datagram handled. ip: " + e.SessionBaseInfo.IP);
        }

        private void SocketServer_DatabaseOpenException(object sender, TExceptionEventArgs e)
        {
            this.tb_DatabaseExceptionCount.Text = m_socketServer.DatabaseExceptionCount.ToString();
            this.AddInfo("open database exception: " + e.ExceptionMessage);
        }

        private void SocketServer_DatabaseCloseException(object sender, TExceptionEventArgs e)
        {
            this.tb_DatabaseExceptionCount.Text = m_socketServer.DatabaseExceptionCount.ToString();
            this.AddInfo("close database exception: " + e.ExceptionMessage);
        }

        private void SocketServer_DatabaseException(object sender, TExceptionEventArgs e)
        {
            this.tb_DatabaseExceptionCount.Text = m_socketServer.DatabaseExceptionCount.ToString();
            this.AddInfo("operate database exception: " + e.ExceptionMessage);
        }

        private void SocketServer_ShowDebugMessage(object sender, TExceptionEventArgs e)
        {
            this.AddInfo("debug message: " + e.ExceptionMessage);
        }

        private void AddInfo(string message)
        {
            if (lb_ServerInfo.Items.Count > 1000)
            {
                lb_ServerInfo.Items.Clear();
            }

            lb_ServerInfo.Items.Add(message);
            lb_ServerInfo.SelectedIndex = lb_ServerInfo.Items.Count - 1;
            lb_ServerInfo.Focus();
        }
    }

    /// <summary>
    /// �����ûỰSession��
    /// </summary>
    public class TTestSession : TSessionBase
    {
        //private Socket m_socket;
        private int m_maxDatagramSize;

        private BufferManager m_bufferManager;

        private int m_bufferBlockIndex;
        private byte[] m_receiveBuffer;
        private byte[] m_sendBuffer;

        private byte[] m_datagramBuffer;

        private TDatabaseBase m_databaseObj;
        private Queue<byte[]> m_datagramQueue;


        /// <summary>
        /// ��д��������, ������Ϣ���ͻ���
        /// </summary>
        protected override void OnDatagramDelimiterError()
        {
            base.OnDatagramDelimiterError();
            
            base.SendDatagram("datagram delimiter error");
        }

        /// <summary>
        /// ��д��������, ������Ϣ���ͻ���
        /// </summary>
        protected override void OnDatagramOversizeError()
        {
            base.OnDatagramOversizeError();

            base.SendDatagram("datagram over size");
        }

        /*protected override void ResolveSessionBuffer(int readBytesLength)
        {
            // �ϴ����°��ķǿ�, ��Ȼ����ʼ�ַ�<
            bool hasHeadDelimiter = (m_datagramBuffer != null);

            int headDelimiter = 1;
            int tailDelimiter = 1;

            int bufferOffset = m_bufferManager.GetReceivevBufferOffset(m_bufferBlockIndex);
            int start = bufferOffset;   // m_receiveBuffer �������а���ʼλ��
            int length = 0;  // �Ѿ������Ľ��ջ���������

            int subIndex = bufferOffset;  // �������±�
            while (subIndex < readBytesLength + bufferOffset)
            {
                if (m_receiveBuffer[subIndex] == 0xA5 && m_receiveBuffer[subIndex+1] == 0xA5)  // ���ݰ���ʼ�ַ�<��ǰ���������
                {
                    if (hasHeadDelimiter || length > 0)  // ��� < ǰ�������ݣ�����Ϊ�����
                    {
                        this.OnDatagramDelimiterError();
                    }

                    m_datagramBuffer = null;  // ��հ�����������ʼһ���µİ�

                    start = subIndex;         // �°���㣬��<����λ��
                    length = headDelimiter;   // �°��ĳ��ȣ���<��
                    hasHeadDelimiter = true;  // �°��п�ʼ�ַ�
                }
                else if (m_receiveBuffer[subIndex-1] == 0x5A && m_receiveBuffer[subIndex] == 0x5A)  // ���ݰ��Ľ����ַ�>
                {
                    if (hasHeadDelimiter)  // �������������п�ʼ�ַ�<
                    {
                        length += tailDelimiter;  // ���Ȱ��������ַ���>��

                        this.GetDatagramFromBuffer(start, length); // >ǰ���Ϊ��ȷ��ʽ�İ�

                        start = subIndex + tailDelimiter;  // �°���㣨һ��һ�δ�������ѭ����
                        length = 0;  // �°�����
                    }
                    else  // >ǰ��û�п�ʼ�ַ�����ʱ��Ϊ�����ַ�>Ϊһ���ַ����������Ĵ��������
                    {
                        length++;  //  hasHeadDelimiter = false;
                    }
                }
                else  // ���� < Ҳ�� >�� ��һ���ַ������� + 1
                {
                    length++;
                }
                ++subIndex;
            }

            if (length > 0)  // ʣ�µĴ����������������
            {
                int mergedLength = length;
                if (m_datagramBuffer != null)
                {
                    mergedLength += m_datagramBuffer.Length;
                }

                // ʣ�µİ��ĺ����ַ��Ҳ�������ת�浽���Ļ������У����´δ���
                if (hasHeadDelimiter && mergedLength <= m_maxDatagramSize)
                {
                    //this.CopyToDatagramBuffer(start, length);

                    int datagramLength = 0;
                    if (m_datagramBuffer != null)
                    {
                        datagramLength = m_datagramBuffer.Length;
                    }

                    Array.Resize(ref m_datagramBuffer, datagramLength + length);  // �������ȣ�m_datagramBuffer Ϊ null ������
                    Array.Copy(m_receiveBuffer, start, m_datagramBuffer, datagramLength, length);  // ���������ݰ�������
                }
                else  // �������ַ��򳬳�
                {
                    this.OnDatagramOversizeError();
                    m_datagramBuffer = null;  // ����ȫ������
                }
            }
        }*/


        /// <summary>
        /// ��д AnalyzeDatagram ����, �������ݴ洢����
        /// </summary>
        protected override void AnalyzeDatagram(byte[] datagramBytes)
        {
            string msg = "";
            int bytesRead = datagramBytes.Length;

            switch (datagramBytes[21])
            {
                case 0x22:
                    if (datagramBytes[9] == 0xAA)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--AD������ʼ" + "\n";
                    }
                    else if (datagramBytes[9] == 0x55)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--AD��������" + "\n";
                    }
                    break;

                case 0x25:
                    if (datagramBytes[9] == 0x55)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�趨GPS����ʱ��ɹ�" + "\n";
                    }

                    break;

                case 0x26:
                    if (datagramBytes[7] == 0x01)
                    {
                        msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�趨����ʱ���͹ر�ʱ���ɹ�" + "\n";

                    }
                    break;

                case 0x27:
                    int[] gpsData = new int[23];
                    for (int i = 0; i < 23; i++)
                    {
                        gpsData[i] = datagramBytes[9 + i];
                    }
                    //gpsDistance.getGPSData(gpsData, out dataitem.Latitude, out dataitem.Longitude);
                    //msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--����Ϊ��" + dataitem.Longitude + "γ��Ϊ��" + dataitem.Latitude + "\n";

                    break;

                case 0x29:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�趨������IP�ɹ�" + "\n";
                    break;

                case 0x30:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�趨�������˿ںųɹ�" + "\n";

                    break;

                case 0x31:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�趨AP���Ƴɹ�" + "\n";

                    break;

                case 0x32:
                    msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�趨AP����ɹ�" + "\n";

                    break;

                case 0x23:
                    /*if (bytesRead == perPackageLength)
                    {
                        if (dataitem.isSendDataToServer == true)
                        {
                            dataitem.currentsendbulk++;

                            ShowProgressBar(null);

                            for (int i = 7; i < perPackageLength - 2; i++)//���ϴ��İ�ȥ��ͷ��β�������ֽں���ʱ�洢��TotalData[]��
                            {
                                dataitem.byteAllData[dataitem.datalength++] = datagramBytes[i];
                            }

                            if (dataitem.datalength == g_datafulllength)//1000*600 = 600000;
                            {
                                StoreDataToFile(dataitem.intDeviceID, dataitem.byteAllData);

                                dataitem.currentsendbulk = 0;
                                dataitem.isSendDataToServer = false;
                                dataitem.CmdStage = 3;

                                msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + "" + "�豸��--" + ""+ "--�����ϴ����" + "\n";
                                Console.WriteLine(msg);
                                ShowMsg(msg);
                            }
                        }
                        else
                        {
                            for (int i = 368, j = 0; i <= 373; i++, j++)//���ϴ��İ�ȥ��ͷ��β�������ֽں���ʱ�洢��TotalData[]��
                            {
                                dataitem.byteTimeStamp[j] = (byte)(Convert.ToInt32(datagramBytes[i]) - 0x30);
                            }
                            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "Ӳ��" + dataitem."" + "�豸��--" + ""+ "--ʱ�����:" + byteToHexStr(dataitem.byteTimeStamp) + "\n";
                            Console.WriteLine(msg);
                            ShowMsg(msg);
                        }
                    }*/
                    break;

                case 0xFF:
                    msg = "�յ�������";
                    /*if (""== 0)//ֻ�ж��µ�ַ���������������ظ����
                    {
                        //�豸��ID�ַ���
                        ID[0] = datagramBytes[3];
                        ID[1] = datagramBytes[4];
                        ID[2] = datagramBytes[5];
                        ID[3] = datagramBytes[6];
                        intdeviceID = byteToInt(ID);

                        string oldAddress = checkIsHaveID(intdeviceID);//�õ���ǰID��Ӧ�ľɵ�ַ

                        if (oldAddress != null)//�����ڣ��Ѿɵ�ַ�����Ը��Ƶ��µ�ַ��
                        {//���������ڵ��ߣ���dataitem����Ҫ�̳о��豸��ֻ��Ҫ�����������ԣ���IP��port��socket��
                            DataItem olddataitem = (DataItem)htClient[oldAddress];//ȡ����ǰ����IP��Ӧ��dataitem

                            dataitem.strIP = strIP;
                            dataitem.strPort = strPort;
                            dataitem.socket = clientSocket;
                            datagramBytes = new byte[perPackageLength];
                            dataitem."" = "";

                            dataitem.datalength = olddataitem.datalength;//�̳о�����
                            dataitem.byteAllData = olddataitem.byteAllData;//�̳о�����
                            dataitem.currentsendbulk = olddataitem.currentsendbulk;//�̳о�����

                            dataitem.byteDeviceID = ID;
                            ""= intdeviceID;

                            dataitem.isSendDataToServer = olddataitem.isSendDataToServer;//�̳о�����
                            dataitem.isChoosed = false;
                            dataitem.CmdStage = olddataitem.CmdStage;//�̳о�����
                            dataitem.uploadGroup = 0;

                            dataitem.byteTimeStamp = olddataitem.byteTimeStamp;//ʱ������̳о�����
                            dataitem.Longitude = olddataitem.Longitude;//���ȣ����Σ��̳о�����
                            dataitem.Latitude = olddataitem.Latitude;//γ�ȣ� ǰ��Σ��̳о�����

                            htClient.Remove(oldAddress);//ɾ���ɵ�ַ�ļ�ֵ��
                            string OldAddress = oldAddress + "--" + dataitem.intDeviceID.ToString();
                            RemoveAddress(OldAddress);

                            htClient[""] = dataitem;//���豸��IP���豸��dataitem��Ӧ�ظ��½���ϣ��
                            string newAddress = "" + "--" + dataitem.intDeviceID.ToString();
                            AddAddress(newAddress);
                        }
                        else
                        {
                            //�������ڣ�����ȫ�µ�ַ������ID��
                            ""= intdeviceID;
                            dataitem.byteDeviceID = ID;

                            string newAddress = "" + "--" + dataitem.intDeviceID.ToString();
                            AddAddress(newAddress);
                        }
                    }//if (""== 0)*/
                    break;

                default:
                    break;
            }

            Console.WriteLine(msg);

            /*string datagramText = Encoding.ASCII.GetString(datagramBytes);

            string clientName = string.Empty;
            int datagramTextLength = 0;

            int n = datagramText.IndexOf(',');  // ��ʽΪ <C12345,0000000000,****>
            if (n >= 1)
            {
                clientName = datagramText.Substring(1, n - 1);
                try
                {
                    datagramTextLength = int.Parse(datagramText.Substring(n + 1, 10));
                }
                catch
                {
                    datagramTextLength = 0;
                }
            }

            base.OnDatagramAccepted();  // ģ����յ�һ�����������ݰ�

            if (!string.IsNullOrEmpty(clientName) && datagramTextLength > 0)
            {

                if (datagramTextLength == datagramBytes.Length)
                {
                    base.SendDatagram("<OK: " + clientName + ", datagram length = " + datagramTextLength.ToString() + ">");

                    this.Store(datagramBytes);
                    base.OnDatagramHandled();  // ģ���Ѿ������洢�������ݰ�
                }
                else
                {
                    base.SendDatagram("<ERROR: " + clientName + ", error length, datagram length = " + datagramTextLength.ToString() + ">");
                    base.OnDatagramError();  // �����
                }
            }
            else if (string.IsNullOrEmpty(clientName))
            {
                base.SendDatagram("client: no name, datagram length = " + datagramTextLength.ToString());
                base.OnDatagramError();
            }
            else if (datagramTextLength == 0)
            {
                base.SendDatagram("client: " + clientName + ", datagram length = " + datagramTextLength.ToString());
                base.OnDatagramError();  // �����
            }*/
        }

        /// <summary>
        /// �Զ�������ݴ洢����
        /// </summary>
        private void Store(byte[] datagramBytes)
        {
            if (this.DatabaseObj == null)
            {
                return;
            }

            TTestAccessDatabase db = this.DatabaseObj as TTestAccessDatabase;
            db.Store(datagramBytes, this);
        }
    }

    /// <summary>
    /// ������Access���ݿ���
    /// </summary>
    public class TTestAccessDatabase : TOleDatabaseBase
    {
        private OleDbCommand m_command;  // �Զ�����ֶ�
        
        /// <summary>
        /// ��д Open ����
        /// </summary>
        public override void Open()
        {
            base.Open();  // �����ݿ�

            m_command = new OleDbCommand();
            m_command.Connection = (OleDbConnection)this.DbConnection;

            // OleDbCommand ������ SqlCommand �� CommandText ʹ�ò�������
            m_command.CommandText = "insert into DatagramTextTable(SessionIP, SessionName, DatagramSize) values (?, ?, ?)";

            m_command.Parameters.Add(new OleDbParameter("SessionIP",OleDbType.VarChar));
            m_command.Parameters.Add(new OleDbParameter("SessionName", OleDbType.VarChar));
            m_command.Parameters.Add(new OleDbParameter("DatagramSize", OleDbType.Integer));
        }

        /// <summary>
        /// �Զ������ݴ洢����
        /// </summary>
        public override void Store(byte[] datagramBytes, TSessionBase session)
        {
            string datagramText = Encoding.ASCII.GetString(datagramBytes);
            try
            {
                m_command.Parameters["SessionIP"].Value = session.IP;
                m_command.Parameters["SessionName"].Value = session.Name;
                m_command.Parameters["DatagramSize"].Value = datagramBytes.Length;

                m_command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                this.OnDatabaseException(err);
            }
        }
    }
}