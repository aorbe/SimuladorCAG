using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Simulador_CAG;

namespace Simulador_CAG
{

    class Simulador
    {
        // Estações 1 e 2
        FanCoil fc1, fc2;
        Chiller ch1, ch2;

        Timer timer;

        bool recalc = true;

        Random random = new Random();

        int numberConnections = 0;
        int [] local = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        
        private static Simulador _simulador = null;
        private static Simulador_CAG.ModbusServer easyModbusTCPServer;

        public static Simulador instance()
        {
            if (_simulador == null)
            {
                easyModbusTCPServer = new Simulador_CAG.ModbusServer();
                easyModbusTCPServer.Listen();
                _simulador = new Simulador();
                easyModbusTCPServer.HoldingRegistersChanged += 
                    new ModbusServer.HoldingRegistersChangedHandler(_simulador.HoldingRegistersChanged);
            }
            return _simulador;
        }

        delegate void numberOfConnectionsCallback();
        private void NumberOfConnectionsChanged()
        {
            numberConnections =  easyModbusTCPServer.NumberOfConnections;
            ConnectionsChanged(numberConnections);
        }


        private Simulador()
        {
            easyModbusTCPServer.NumberOfConnectedClientsChanged += 
                new ModbusServer.NumberOfConnectedClientsChangedHandler(NumberOfConnectionsChanged);
            fc1 = new FanCoil(random.Next(9000) / 100.0);
            fc1.Tinp = (random.Next(1500, 3000) / 100.0);
            fc2 = new FanCoil(random.Next(9000) / 100.0);
            fc2.Tinp = (random.Next(1500, 3000) / 100.0); 
            ch1 = new Chiller();
            ch1.Tinp = (random.Next(1000, 2000) / 100.0);
            ch2 = new Chiller();
            ch2.Tinp = (random.Next(1000, 2000) / 100.0);

            int period = 1000;
            timer = new Timer(this.atualiza, null, period, period);
        }

        public void Stop()
        {
            timer.Dispose();
        }

        const int TMP_INP_FC1 = 0;
        const int TMP_OUT_FC1 = 1;
        const int POS_VAL_FC1 = 2;
        const int STP_TMP_FC1 = 3;
        const int TMP_INP_FC2 = 4;
        const int TMP_OUT_FC2 = 5;
        const int POS_VAL_FC2 = 6;
        const int STP_TMP_FC2 = 7;

        const int STP_VAL_FC1 = 16;
        const int STP_VAL_FC2 = 17;

        const int TMP_INP_CH1 = 8;
        const int TMP_OUT_CH1 = 9;
        const int STP_TMP_CH1 = 10;

        const int TMP_INP_CH2 = 11;
        const int TMP_OUT_CH2 = 12;
        const int STP_TMP_CH2 = 13;
        //8 & 0.1 oC & Temperatura de Entrada da Água no Chiller 1				& Apenas Leitura \\ \hline
        //9 & 0.1 oC & Temperatura de Saída da Água no Chiller 1 				& Apenas Leitura \\ \hline
        //10 & 0.1 oC & Setpoint de Temperatura do Chiller 1					& Leitura/Escrita \\ \hline
        //11 & 0.1 oC & Temperatura de Entrada da Água no Chiller 2				& Apenas Leitura \\ \hline
        //12 & 0.1 oC & Temperatura de Saída da Água no Chiller 2 				& Apenas Leitura \\ \hline
        //13 & 0.1 oC & Setpoint de Temperatura do Chiller 2					& Leitura/Escrita \\ \hline


        const int CMD_REGISTER = 14;
        const int MSK_CMD_FC1 = 0x0001;
        const int MSK_MAN_FC1 = 0x0002;
        const int MSK_CMD_FC2 = 0x0004;
        const int MSK_MAN_FC2 = 0x0008;
        const int MSK_CMD_CH1 = 0x0010;
        const int MSK_CMD_CH2 = 0x0020;
        const int STS_REGISTER = 15;

        

        //14 & 0 & Comando Liga/Desliga Fan-Coil 1 & Toggle\\ \hline

        //40015 & 4 & Comando Liga/Desliga Chiller 1 & Toggle\\ \hline
        //40015 & 5 & Comando Liga/Desliga Chiller 2 & Toggle\\ \hline

        private void atualiza(object state)
        {
            fc1.Control((local[CMD_REGISTER] & MSK_CMD_FC1)!=0, (local[CMD_REGISTER] & MSK_MAN_FC1)!=0);
            fc2.Control((local[CMD_REGISTER] & MSK_CMD_FC2)!=0, (local[CMD_REGISTER] & MSK_MAN_FC2)!=0);
            ch1.Control((local[CMD_REGISTER] & MSK_CMD_CH1) != 0);
            ch2.Control((local[CMD_REGISTER] & MSK_CMD_CH2) != 0);

            fc1.Tinp += ((random.Next(100) - 50) / 1000.0);
            if (fc1.Tinp > 35.0)
                fc1.Tinp = 35;
            if (fc1.Tinp < 15.0)
                fc1.Tinp = 15;
            fc2.Tinp += ((random.Next(100) - 50) / 1000.0);
            if (fc2.Tinp > 35.0)
                fc2.Tinp = 35;
            if (fc2.Tinp < 15.0)
                fc2.Tinp = 15;


            fc1.Calc(local[STP_TMP_FC1] /10.0, local[STP_VAL_FC1] / 10.0);
            fc2.Calc(local[STP_TMP_FC2] /10.0, local[STP_VAL_FC2] / 10.0);

            int n_chillers = (ch1.state?1:0) + (ch2.state?1:0);
            double q = (fc1.Vpos * fc1.Tinp + fc2.Vpos * fc2.Tinp) * n_chillers;

            double e_Temp = (fc1.Tout + fc2.Tout) / 2 - (ch1.Tinp - ch2.Tinp) / 2;
            ch1.Tout += e_Temp / 10.0;
            ch2.Tout += e_Temp / 10.0;
 
            ch1.Calc(local[STP_TMP_CH1] / 10.0, q);
            ch2.Calc(local[STP_TMP_CH2] / 10.0, q);

            int[] new_values = new int[] { (int)(10.0 * fc1.Tinp), (int)(10.0 * fc1.Tout), (int)(10.0 * fc1.Vpos), local[STP_TMP_FC1],
                (int) (10.0 * fc2.Tinp), (int) (10.0 * fc2.Tout), (int) (10.0 * fc2.Vpos), local[STP_TMP_FC2], (int) (10.0 * ch1.Tinp),
            (int) (10.0 * ch1.Tout), local[STP_TMP_CH1] , (int) (10.0 * ch2.Tinp), (int) (10.0 * ch2.Tout), local[STP_TMP_CH2]};

            for(int x=0; x< new_values.Length; x++)
            {
                if (local[x] != new_values[x])
                {
                    easyModbusTCPServer.changeHoldingRegisters(x, (short)new_values[x]);
                    local[x] = new_values[x];
                }
            }
            int status = 0;
            if (fc1.state) status += 0x01;
            if (fc2.state) status += 0x02;
            if (ch1.state) status += 0x04;
            if (ch2.state) status += 0x08;
            easyModbusTCPServer.changeHoldingRegisters(STS_REGISTER, (short)status);

            if (ProcessChanged != null)
                ProcessChanged(local);

        }

        delegate void registersChangedCallback(int register, int numberOfRegisters);

        public void HoldingRegistersChanged(int register, int numberOfRegisters)
        {
            if (register <= 19)
            {
                for (int x = 0; x < numberOfRegisters; x++)
                {
                    if (x > 19)
                        return;
                    local[register + x] = easyModbusTCPServer.holdingRegisters[register + x];
                    recalc = true;
                }
            }
        }

        public void toggleHoldingRegister(int register, short value)
        {
            easyModbusTCPServer.holdingRegisters[register] ^= value;
            HoldingRegistersChanged(register, easyModbusTCPServer.holdingRegisters[register]);
        }

        #region events
        public delegate void ProcessChangedHandler(int[] registro);
        public event ProcessChangedHandler ProcessChanged;
        public delegate void ConnectionsChangedHandler(int numberConnections);
        public event ConnectionsChangedHandler ConnectionsChanged;
        #endregion
    }


}