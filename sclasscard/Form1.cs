using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace sclasscard
{
    public partial class Form1 : Form
    {
        string cid, cpw, curl, savedid;
        decimal word, num;
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _options = null;
        protected ChromeDriver _driver = null;
        public Form1()
        {
            InitializeComponent();
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileInfo File_Check = new FileInfo(Application.StartupPath + @"\sclassid.tmp");

            if (File_Check.Exists) 
            {
                gunaGoogleSwitch1.Checked = true;
                savedid = System.IO.File.ReadAllText(Application.StartupPath + @"\sclassid.tmp");
                gunaTextBox1.Text = savedid; // 저장된 아이디가 있다면 불러오기
            }

            else
            gunaGoogleSwitch1.Checked = false;
        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            if (gunaTextBox1.Text != "")
            {
                if (gunaTextBox2.Text != "")
                {
                    if (gunaTextBox3.Text != "")
                    {
                        cid = gunaTextBox1.Text; // 아이디
                        cpw = gunaTextBox2.Text; // 비밀번호
                        curl = gunaTextBox3.Text; // 클래스카드 주소

                        word = numericUpDown1.Value; // 단어 개수

                        label5.Text = "Started.";
                        label7.Text = "(1/2) Login..";  // 라벨 변경

                        _driver = new ChromeDriver(_driverService, _options);
                        _driver.Navigate().GoToUrl("http://www.classcard.net/Login"); // 주소값을 텍스트박스의 텍스트로
                        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                        var ids = _driver.FindElementByXPath("/html/body/div[1]/div[2]/div/div/div/form/div[1]/input[1]"); //아이디
                        ids.SendKeys(cid);

                        var pws = _driver.FindElementByXPath("/html/body/div[1]/div[2]/div/div/div/form/div[1]/input[2]"); //비밀번호
                        pws.SendKeys(cpw);

                        var logs = _driver.FindElementByXPath("/html/body/div[1]/div[2]/div/div/div/form/div[2]/button"); //로그인
                        logs.Click();

                        backgroundWorker1.RunWorkerAsync(); //암기 사이트 접속 및 매크로 시작    
                        
                        if (gunaGoogleSwitch1.Checked == true) // ID저장 체크 시
                        {
                            System.IO.File.WriteAllText(Application.StartupPath + @"\sclassid.tmp", gunaTextBox1.Text); // 같은 폴더에 sclassid.tmp로 아이디 평문 저장

                        }
                         else
                        {
                            System.IO.File.WriteAllText(Application.StartupPath + @"\sclassid.tmp", "");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("모든 내용을 확인하십시오.");
            }                      
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
            _driver.SwitchTo().Window(_driver.WindowHandles.Last()); // 로그인 탭에서 암기 탭으로 변경


            //클래스카드 암기 주소로 이동
            _driver.Navigate().GoToUrl(curl);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); // 5초 대기

            num = 0;
            var mean = _driver.FindElementByXPath("/html/body/div[2]/div/div/div[3]/div[1]/a"); // 뜻 보기
            var know = _driver.FindElementByXPath("/html/body/div[2]/div/div/div[3]/div[2]/a"); // 알아요 버튼
            var next = _driver.FindElementByXPath("/html/body/div[2]/div/div/div[2]/div[4]/a"); // 다음 단어 버튼
            var nesett = _driver.FindElementByXPath("/html/body/div[5]/div/div[2]/div[3]/div[3]/a"); // 다음 세트 버튼
            do
            {
                mean.Click(); // 뜻 보기 클릭
                know.Click(); // 이제 알아요 클릭
                next.Click(); // 다음 클릭
                num += 1;
                if ((num % 10) == 0)
                {
                    nesett.Click(); // 다음 클릭

                }

            } while (num <= word);

            // var exit = _driver.FindElementByXPath("/html/body/div[1]/div/div[1]/div[1]/a");
            // var exit2 = _driver.FindElementByXPath("/html/body/div[13]/div[2]/div/div/a[5]");

            //exit.Click();
            //exit2.Click();
            // 아직 구현 안됨.



            

            
        }
    }
}
