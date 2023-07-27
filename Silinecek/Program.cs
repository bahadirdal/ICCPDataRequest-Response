using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Program
{

    // ICCP veri isteği yapısı
    public class IccpDataRequest
    {
        public int DataObjectID { get; set; }
        // Diğer gereken alanlar eklenebilir...
    }

    // ICCP veri yanıtı yapısı
    public class IccpDataResponse
    {
        public int DataObjectID { get; set; }
        public byte[] Data { get; set; }
        // Diğer gereken alanlar eklenebilir...
    }

    static void Main()
    {
        // Sunucunun IP adresi ve port numarası
        string serverIP = "192.168.0.100";
        int serverPort = 12345;

        try
        {
            // TCP/IP bağlantısı oluşturma ve sunucuya bağlanma
            using (var client = new TcpClient())
            {
                client.Connect(serverIP, serverPort);
                Console.WriteLine("Sunucuya bağlandı.");

                // ICCP veri isteği oluşturma ve gönderme
                IccpDataRequest request = new IccpDataRequest
                {
                    DataObjectID = 123 // Örnek bir veri nesnesi ID'si
                };

                // IccpDataRequest nesnesini byte dizisine çevirme
                byte[] requestBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, request);
                    requestBytes = memoryStream.ToArray();
                }

                NetworkStream stream = client.GetStream();
                stream.Write(requestBytes, 0, requestBytes.Length);
                Console.WriteLine("ICCP veri isteği gönderildi.");

                // Sunucudan ICCP veri yanıtı alma
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                // Gelen byte dizisini IccpDataResponse nesnesine çevirme
                IccpDataResponse response;
                using (MemoryStream memoryStream = new MemoryStream(buffer, 0, bytesRead))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    response = (IccpDataResponse)binaryFormatter.Deserialize(memoryStream);
                }

                Console.WriteLine("Sunucudan ICCP veri yanıtı alındı. DataObjectID: " + response.DataObjectID);
                // Yanıtta alınan verileri kullanabilirsiniz: response.Data
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata oluştu: " + ex.Message);
        }

        Console.ReadLine();
    }
}

