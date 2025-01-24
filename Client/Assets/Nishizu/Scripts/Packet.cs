using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UdpBuffer
{
    private readonly object _lockObject = new object();
    private byte[] _buffer = null;

    public object LockObject => _lockObject;
    public byte[] Buffer { get => _buffer; set => _buffer = value; }
}

public class PacketData
{
    [Flags]
    public enum eInputMask : byte
    {
        Throw = 1 << 0,
        Jump = 1 << 1,
        // ChangeCharacter = 1 << 2,
    }

    [Flags]
    public enum eStateMask : byte
    {
        Ground = 1 << 3,
        Reset = 1 << 4,
    }

    private byte _timer = 0;
    private eInputMask _inputMask = 0;
    private Vector3 _force = new Vector3();

    public byte Timer { get { return _timer; } }
    public eInputMask InputMask { get { return _inputMask; } }
    public Vector3 Force { get { return _force; } }

    public PacketData()
    {
    }

    public PacketData(byte timer, eInputMask inputMask, Vector3 force)
    {
        _timer = timer;
        _inputMask = inputMask;
        _force = force;
    }

    public byte[] GetBytes()
    {
        byte[] data0 = { _timer };
        byte[] data1 = { (byte)_inputMask };
        byte[] data2 = System.BitConverter.GetBytes(_force.x);
        byte[] data3 = System.BitConverter.GetBytes(_force.y);
        byte[] data4 = System.BitConverter.GetBytes(_force.z);

        byte[] bytes = new byte[data0.Length + data1.Length + data2.Length + data3.Length + data4.Length];

        int offset = 0;
        Array.Copy(data0, 0, bytes, offset, data0.Length); offset += data0.Length;
        Array.Copy(data1, 0, bytes, offset, data1.Length); offset += data1.Length;
        Array.Copy(data2, 0, bytes, offset, data2.Length); offset += data2.Length;
        Array.Copy(data3, 0, bytes, offset, data3.Length); offset += data3.Length;
        Array.Copy(data4, 0, bytes, offset, data4.Length); offset += data4.Length;

        return bytes;
    }

    public int ReadBytes(byte[] bytes, int startIndex)
    {
        _timer = bytes[startIndex]; startIndex += sizeof(byte);
        _inputMask = (eInputMask)bytes[startIndex]; startIndex += sizeof(byte);

        float x = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);
        float y = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);
        float z = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);

        _force.x = x;
        _force.y = y;
        _force.z = z;

        return startIndex;
    }
}

public class Packet
{
    private PacketData[] _datas = new PacketData[3];

    public void Push(byte timer, PacketData.eInputMask inputMask, Vector3 force)
    {
        int i;

        for (i = 0; i < _datas.Length - 1; i++)
        {
            if (_datas[i + 1] == null)
                break;

            _datas[i] = _datas[i + 1];
        }

        for (; i < _datas.Length; i++)
        {
            _datas[i] = new PacketData(timer, inputMask, force);
        }
    }

    public byte[] GetBytes()
    {
        byte[] packet = _datas[0].GetBytes();
        byte[] buffer = new byte[_datas.Length * packet.Length];

        int offset = 0;

        Array.Copy(packet, 0, buffer, offset, packet.Length); offset += packet.Length;

        for (int i = 1; i < _datas.Length; i++)
        {
            packet = _datas[i].GetBytes();
            Array.Copy(packet, 0, buffer, offset, packet.Length); offset += packet.Length;
        }

        return buffer;
    }
}
