using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
namespace SLR_parser {
    public partial class Form1 : Form {

        public List<List<String>> Table = new List<List<string>>();
        public List<List<string>> tokens = new List<List<string>>();
        public SLR_DFA dfa;

    public string test;

        public Form1() {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e) {

            String grammar = InputBox.Text;

            Preprocessor preprocessor1 = new Preprocessor();
            Tuple<int, String> output = preprocessor1.InitGrammar(grammar);

            
            preprocessor1.Find_FirstSet();
            preprocessor1.Find_FollowSet();
           

           

            dfa = new SLR_DFA(preprocessor1.Rules, preprocessor1.nonterminals, preprocessor1.Start_symbol);

            dfa.augmentGrammar(preprocessor1.Rules, preprocessor1.nonterminals, preprocessor1.Start_symbol);
            dfa.statesDict[0] = new List<List<List<String>>>(dfa.findClosure(dfa.AugRules, dfa.AugRules[0][0][0]));
            dfa.generateStates(dfa.statesDict);
            Table = dfa.createParseTable(dfa.statesDict, dfa.stateMap, preprocessor1.terminals, preprocessor1.nonterminals, preprocessor1.FOLLOW_SET);

            //// Display GOTO in GUI
            foreach (var item in dfa.stateMap)
            {
                String val = "GOTO: ( I" + item.Key.Item1 + " , " + item.Key.Item2 + " ) = I" + item.Value + "\n";
                Console.WriteLine(val);
                
            }

            Console.WriteLine("\n--------------------------------------------------\n");

            // Display DFA
            Console.WriteLine("\n\n -------------------- Deterministic Finite Automata------------\n\n");
            foreach (var st in dfa.statesDict) {
                Console.WriteLine("\nSTATE : "+st.Key+"\n");
               
                foreach (var item in dfa.statesDict[st.Key]) {
                    Console.WriteLine("  "+ String.Join(" ", item[0]) + " -> "+String.Join(" ",item[1]));
     
                }
                Console.WriteLine("\n");
               
            }

            
            int i = 1;
            
            Console.WriteLine("-----------------------Parsing Table---------------\n");
            Console.WriteLine("------------------Action---------------------------------Goto------------------\n");
            Console.Write("  ");
            foreach (var x in dfa.colss) {
                Console.Write(x+"  ");
                
                i++;
            }
            Console.WriteLine("\n");

            int count = 0;
            foreach (var x in Table) {
                Console.WriteLine(count + ": " + String.Join("   ", x));
                x.Insert(0, "I" + count);
                
                count++;
            }

            
        }


        private void ClearAll_Click(object sender, EventArgs e) {
            InputBox.Clear();   
            InputString.Clear();
            
        }

        private void Ptable_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }

        private void ParseButton_Click(object sender, EventArgs e) {
            
            String grammar = InputBox.Text;
            String input = InputString.Text;

           

         

            

            InputParsing parser = new InputParsing(Table, dfa.numbered_rules);
            List<List<String>> outputTrace = parser.parse(InputString.Text, dfa.colss);

           
            Console.WriteLine("------------------------Parsing stack --------------------\n");
            Console.WriteLine("Stack                    Input                   Action");
            foreach (var x in outputTrace) {
                String[] data  = x.ToArray();
                String stack = data[0];
                String Input = data[1];
                String action = data[2];
                Console.WriteLine(stack+"                    "+Input+"                    "+action);
               
            }

            // Generate Annotated Parse Tree
            SemanticAnalyzer SDT = new SemanticAnalyzer(InputString.Text, "");
            //Displaying the sematic rules
            Console.WriteLine("-----------------Attribute values--------------");
            Console.WriteLine(SDT.generateAnnotatedPTree());
           

            // Generate Syntax Tree
            MainSyntaxTree MST = new MainSyntaxTree();
            // converting the expression from infix form to postfix form inorder to make it easier to display

            Console.WriteLine("------------------------------------------");
            String expr = MST.InfixToPostfix(InputString.Text);
            
            String[] arrPostfix = expr.Split(' ');

            
            String tree = MST.GenerateSyntaxTree(arrPostfix);
            Console.WriteLine("\n-------------------------Parse Tree ---------------------");

            Console.WriteLine(tree);
          

            //Generate 3 addr code
            ThreeAddrCode IR = new ThreeAddrCode();
            String code = IR.GenerateCode(MST.getRoot(arrPostfix));
            Console.WriteLine("-------------------Three Address Code ------------------------");
            Console.WriteLine(code);
            
        }

       

        private void tabPage3_Click(object sender, EventArgs e) {

        }

        private void groupBox7_Enter(object sender, EventArgs e) {

        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    InputBox.Text = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }
    }
}
