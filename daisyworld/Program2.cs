using System;

namespace daisyworld{
	public class Program2
	{
		Program2()
		{
			Daisyworld dw = new Daisyworld(30,30);
			
			for(int i=30;i!=0;i--){
				dw.refresh();
				Console.Clear();
				Console.Write(dw.ToString());
				System.Threading.Thread.Sleep(1000);
			}
		}
		static void Main(string[] args)
		{
			new Program2();
		}
	}
}
