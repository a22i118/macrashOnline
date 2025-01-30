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
        Jump = 1 << 0,
        Throw = 1 << 1,
        PickUp = 1 << 2,
        Menu = 1 << 3,
    }

    [Flags]
    public enum eStateMask : byte
    {
        Ground = 1 << 4,
    }

    private byte _timer = 0;
    private eInputMask _inputMask = 0;
    private Vector2 _movement = new Vector2();

    public byte Timer { get { return _timer; } }
    public eInputMask InputMask { get { return _inputMask; } }
    public Vector2 Movement { get { return _movement; } }

    public PacketData()
    {
    }

    public PacketData(byte timer, eInputMask inputMask, Vector2 movement)
    {
        _timer = timer;
        _inputMask = inputMask;
        _movement = movement;
    }

    public byte[] GetBytes()
    {
        byte[] data0 = { _timer };
        byte[] data1 = { (byte)_inputMask };
        byte[] data2 = System.BitConverter.GetBytes(_movement.x);
        byte[] data3 = System.BitConverter.GetBytes(_movement.y);

        byte[] bytes = new byte[data0.Length + data1.Length + data2.Length + data3.Length];

        int offset = 0;
        Array.Copy(data0, 0, bytes, offset, data0.Length); offset += data0.Length;
        Array.Copy(data1, 0, bytes, offset, data1.Length); offset += data1.Length;
        Array.Copy(data2, 0, bytes, offset, data2.Length); offset += data2.Length;
        Array.Copy(data3, 0, bytes, offset, data3.Length); offset += data3.Length;

        return bytes;
    }

    public int ReadBytes(byte[] bytes, int startIndex)
    {
        _timer = bytes[startIndex]; startIndex += sizeof(byte);
        _inputMask = (eInputMask)bytes[startIndex]; startIndex += sizeof(byte);

        float x = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);
        float y = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);

        _movement.x = x;
        _movement.y = y;
        return startIndex;
    }
}

public class Packet
{
    private PacketData[] _datas = new PacketData[3];

    public void Push(byte timer, PacketData.eInputMask inputMask, Vector2 movement)
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
            _datas[i] = new PacketData(timer, inputMask, movement);
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
