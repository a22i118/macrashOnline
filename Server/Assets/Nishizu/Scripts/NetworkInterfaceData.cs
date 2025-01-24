using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

public class NetworkInterfaceData
{
    public string InterfaceName;
    public IPAddress IpAddress;
    public PhysicalAddress MacAddress;

    public NetworkInterfaceData(string InterfaceName, IPAddress IpAddress, PhysicalAddress MacAddress)
    {
        this.InterfaceName = InterfaceName;
        this.IpAddress = IpAddress;
        this.MacAddress = MacAddress;
    }

    /// <summary>
    /// ネットワーク機器のリストを返す
    /// </summary>
    /// <returns>List<NetworkInterfaceData></returns>
    public static List<NetworkInterfaceData> GetIpAddress()
    {
        List<NetworkInterfaceData> networkInterfaces = new List<NetworkInterfaceData>();

        // NetworkInterfaceを取得
        NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var ni in nis)
        {
            IPInterfaceProperties ipip = ni.GetIPProperties();
            PhysicalAddress physicalAddress = ni.GetPhysicalAddress();
            UnicastIPAddressInformationCollection uipaic = ipip.UnicastAddresses;

            // Debug.Log("接続名:" + ni.Name);
            // Debug.Log("説明:" + ni.Description);
            // Debug.Log("種類:" + ni.NetworkInterfaceType);
            // Debug.Log("速度:" + ni.Speed);
            // Debug.Log("MAC(物理)アドレス:" + ni.GetPhysicalAddress());

            foreach (var uipai in uipaic)
            {
                if (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    uipai.Address.AddressFamily.ToString() == "InterNetwork" &&
                    ni.Name != "lo" &&
                    ni.Name != "lo0" &&
                    uipai.Address.ToString().IndexOf("169.254") == -1)  // Link Local Address は除く
                {
                    networkInterfaces.Add(new NetworkInterfaceData(ni.Name, uipai.Address, physicalAddress));
                }
            }
        }

        return networkInterfaces;
    }
}
