using System;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;

namespace ChatTupla.TupleSpaceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SpaceRepository repository = new SpaceRepository();
            string ip = null;
            string port = null;
            if(args.Length == 1){
                ip = args[0];
                port = "8989";
                repository.AddGate($"tcp://{ip}:8989?KEEP");
            }

            else if(args.Length == 2){
                ip = args[0];
                port = args[1];
                repository.AddGate($"tcp://{ip}:{port}?KEEP");
            }
            else{
                Console.WriteLine("Nenhum IP ou porta foi dado!");
                Console.WriteLine("Configuração default será usada!");
                ip = "127.0.0.1";
                port = "8989";
                repository.AddGate("tcp://127.0.0.1:8989?KEEP");
            }

            repository.AddSpace("chat", new SequentialSpace());

            Servidor s = new Servidor(ip, port, repository);



            // string ip = null;
            // string port = null;
            // if(args.Length == 1){
            //     ip = args[0];
            //     port = "8989";
            // }
            // else if(args.Length == 2){
            //     ip = args[0];
            //     port = args[1];
            // }
            // else{
            //     ip = "127.0.0.1";
            //     port = "8989";
            // }
            // Servidor s = new Servidor(ip, port);

            // // Console.ReadKey();
            // while(true){Thread.Sleep(100);}

            Console.Read();
        }
    }
}
