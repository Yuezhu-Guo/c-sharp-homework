using System;
using System.Windows.Forms;

namespace ATM
{
    public partial class Form1 : Form
    {
        public static Account a = new Account(); 
        public static CreditAccount ca = new CreditAccount();
        public Form1()
        {
            InitializeComponent();
        }
        public static string name, pass;
        private string login_input_ID, login_input_pass;
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            name = pass = null;
            f2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(login_input_ID)&&!String.IsNullOrEmpty(login_input_pass))
            {
                Form3 f3 = new Form3();
                if (login_input_ID == ca[0] && login_input_pass == ca.password)
                    f3.Show();
                else
                    MessageBox.Show("wrong ID or password." + "\r\n" + "Your ID is :" + ca[0]);
                    
            }
            else
            {                           //use custom Exception, Error number 0
                try
                {
                    Account.emptyInput(login_input_ID);
                    Account.emptyInput(login_input_pass);
                }
                catch (EmptyException ex)
                {
                    MessageBox.Show(ex.Message + ex.getId());
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            login_input_ID = this.textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            login_input_pass = this.textBox2.Text;
        }
    }

    //BigMoneyArgs - class
    public class BigMoneyArgs : EventArgs 
    {
        public int ID;
        public int amount_fetch;
    }
    public delegate void BigMoneyHandler(object sender, BigMoneyArgs e);

    //defining exception classes
    class SurpassException : ApplicationException
    {
        private int idnumber;
        public SurpassException(String message, int id) : base(message)
        {
            this.idnumber = id;
        }
        public int getId()
        {
            return idnumber;
        }
    }
    class EmptyException : ApplicationException
    {
        private int idnumber;
        public EmptyException(String message, int id): base(message)
        {
            this.idnumber = id;
        }
        public int getId()
        {
            return idnumber;
        }
    }

    //BadCashException - exception class
    class BadCashException : ApplicationException
    {
        private int idnumber;
        public BadCashException(String message, int id) : base(message)
        {
            this.idnumber = id;
        }
        public int getId()
        {
            return idnumber;
        }
    }

    //Account - class
    public class Account
    {
        public event BigMoneyHandler BigMoneyFetched; //BigMoneyFetched Event
        public Account() { this.BigMoneyFetched += ShowWarning; }    //add events
        //fields
        protected static long ID = 20183020;
        public string password;
        protected long total_amount = 1000000;       
        protected string[] data = new string[5];
        protected string[] keys =
        {
            "card ID","holder's name", "total sum", "latest withdraw","latest deposit"
        };
        //property
        public long latest_withdraw { set; get; }
        public long latest_deposit { set; get; }
        public string date_withdraw { set; get; }
        public string date_deposit { set; get; }
        //indexer
        public string this[int i]
        {
            set
            {
                data[i] = value;
            }
            get
            {
                if (i >= 0 && i < data.Length)
                    return data[i];
                return null;
            }
        }
        public string this[string key]
        {
            get
            {
                return this[FindIndex(key)];
            }
        }
        private int FindIndex(string key)
        {
            for (int i = 0; i < keys.Length; i++)
                if (keys[i] == key)
                    return i;
            return -1;
        }

        public static void ShowWarning(object sender, BigMoneyArgs e)
        {
            MessageBox.Show($"Too much fetched, swindle warning.\nID:{e.ID:###}\nAmount Fetched:{e.amount_fetch:######}");
        }

        //Exception method
        public void surpass(int amount)
        {
            if (amount > this.total_amount)
            {
                throw new SurpassException("withdraw failed.\nError number:", 0);
            }
        }
        public static void emptyInput(String s)
        {
            if (String.IsNullOrEmpty(s))
                throw new EmptyException("Empty Input is not allowed.\nError number: ", 1);
        }

        //badCash
        public void badCash(int rnd)
        {
            if (rnd < 1)
                throw new BadCashException("Bad Cash Refuse! \nError number: ", 2);
        }
        //methods
        public void create_account(string name, string pass)
        {
            this.password = pass;
            this[0] = Convert.ToString(ID);
            this[1] = name;
            this[2] = Convert.ToString(total_amount);
            this[3] = this[4] = "0";
            MessageBox.Show("Remember your ID:\n" + ID + "\nYou'll use it to log in.");
            ID++;
            this.latest_deposit = this.latest_withdraw = 0;

        }
        public void withdraw(int withdraw_amount)
        {
            if (withdraw_amount <= total_amount&&withdraw_amount>0)
            {
                latest_withdraw = withdraw_amount;
                total_amount -= withdraw_amount;
                this[2] = Convert.ToString(total_amount);
                date_withdraw = DateTime.Now.ToString();
                this[3] = Convert.ToString(withdraw_amount);

                if (withdraw_amount > 10000)        //trigger Event
                {
                    if (BigMoneyFetched != null)
                    {
                        BigMoneyArgs args = new BigMoneyArgs();
                        args.amount_fetch = withdraw_amount;
                        args.ID = Convert.ToInt32(this[0]);
                        BigMoneyFetched(this, args);
                    }
                }                                                                                       //Exception BadCash
                try
                {   
                    Random rnd = new Random();
                    badCash(rnd.Next(0, 3)); //bad cash probability: 25%
                }catch(BadCashException eb)
                {
                    MessageBox.Show(eb.Message + eb.getId() + "\nWithdraw failed.");
                }
            }
            else
            {   //Exception Surpass
                try
                {
                    surpass(withdraw_amount);
                }catch(SurpassException e)
                {
                    MessageBox.Show(e.Message + e.getId());
                }                                                                               
                //Exception EmptyInt
                try
                {
                    Account.emptyInput(Form3.wd);
                }
                catch (EmptyException ex)
                {
                    MessageBox.Show(ex.Message + ex.getId());
                }
            }
        }
        public void deposit(int deposit_amount)
        {
            if (deposit_amount > 0)
            {
                latest_deposit = deposit_amount;
                total_amount += deposit_amount;
                this[2] = Convert.ToString(total_amount);
                date_deposit = DateTime.Now.ToString();
                this[4] = Convert.ToString(deposit_amount);
                try                                                                                    //Exception BashCash
                {
                    Random rnd = new Random();
                    badCash(rnd.Next(0, 2));
                }
                catch (BadCashException eb)
                {
                    MessageBox.Show(eb.Message + eb.getId() + "\nDeposit failed.");
                }
            }
            else
            {                                                                                   
                //Exception EmptyInput
                try
                {
                    Account.emptyInput(Form3.dp);
                }
                catch (EmptyException ex)
                {
                    MessageBox.Show("Empty Input is not allowed.\nError number: " + ex.getId());
                }
            }
        }
        
    }

    //CreditAccount - inheritance class from Account
    public class CreditAccount : Account 
    {
        new public event BigMoneyHandler BigMoneyFetched;
        public CreditAccount() { this.BigMoneyFetched += ShowWarning; }
        private int line_of_credit;        
        public int credit_rating { set; get; }       
        public int get_line_of_credit() //line of credit       
        {
            if (credit_rating == 3 || credit_rating == 2)
                line_of_credit = 50000;
            else if (credit_rating == 1 || credit_rating == 0)
                line_of_credit = 10000;
            else
                line_of_credit = 0;
            return line_of_credit;
        }

        //override method
        new public void surpass(int amount)
        {
            if (amount > this.total_amount+this.get_line_of_credit())
            {
                throw new SurpassException("upper bound surpass", 0);
            }
        }
        new public void create_account(string pass, string name)
        {
            this.password = pass;
            this[0] = Convert.ToString(ID);
            this[1] = name;
            this[2] = Convert.ToString(total_amount);
            this[3] = this[4] = "0";
            ID++;
            this.latest_deposit = this.latest_withdraw = 0;
            this.credit_rating = 3;
            this.get_line_of_credit();
        }
        new public void withdraw(int withdraw_amount)
        {
            latest_withdraw = withdraw_amount;
            if (withdraw_amount <= total_amount + line_of_credit && withdraw_amount > 0)
            {
                total_amount -= withdraw_amount;
                this[2] = Convert.ToString(total_amount);
                date_withdraw = DateTime.Now.ToString();
                this[3] = Convert.ToString(withdraw_amount);
                if (latest_withdraw > total_amount)
                {
                    credit_rating--;
                    get_line_of_credit();
                }
                if (withdraw_amount > 10000)      //trigger Event
                {
                    if (BigMoneyFetched != null)
                    {
                        BigMoneyArgs args = new BigMoneyArgs();
                        args.amount_fetch = withdraw_amount;
                        args.ID = Convert.ToInt32(this[0]);
                        BigMoneyFetched(this, args);
                    }
                }
            }
            else
            {   //Exception surpass
                try
                {
                    surpass(withdraw_amount);
                }
                catch (SurpassException e)
                {
                    MessageBox.Show("withdraw failed.\nError number: " + e.getId());
                } 
                //Exception EmptyInput
                try
                {
                    Account.emptyInput(Form3.wd);
                }
                catch (EmptyException ex)
                {
                    MessageBox.Show("Empty Input is not allowed.\nError number: " + ex.getId());
                }
            }
        }
    }
}
