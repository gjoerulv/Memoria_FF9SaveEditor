using System;
using System.IO;

namespace Memoria.ReUtils
{

    public class DataManager
    {
        protected class MetaData
        {
            public char[] Header = new char[]
            {
            'S',
            'A',
            'V',
            'E'
            };
            public float SaveVersion = 1f;
            public int DataSize;
            public static int SystemAchievementStatusesSize = 1;
            public int LatestSlot = -1;
            public int LatestSave = -1;
            public double LatestTimestamp = -1.0;
            public int IsGameFinishFlag;
            public int SelectedLanguage = -1;
            public sbyte IsAutoLogin;
            public byte[] SystemAchievementStatuses = new byte[DataManager.MetaData.SystemAchievementStatusesSize];
            public byte ScreenRotation;
            public void WriteAllFields(Stream stream, BinaryWriter writer, DataAesEncryption encryption)
            {
                byte[] bytes = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                    {
                        binaryWriter.Write(this.Header, 0, 4);
                        binaryWriter.Write(this.SaveVersion);
                        binaryWriter.Write(this.DataSize);
                        binaryWriter.Write(this.LatestSlot);
                        binaryWriter.Write(this.LatestSave);
                        binaryWriter.Write(this.LatestTimestamp);
                        binaryWriter.Write(this.IsGameFinishFlag);
                        binaryWriter.Write(this.SelectedLanguage);
                        binaryWriter.Write(this.IsAutoLogin);
                        binaryWriter.Write(this.SystemAchievementStatuses);
                        binaryWriter.Write(this.ScreenRotation);
                        byte[] array = new byte[249];
                        binaryWriter.Write(array, 0, array.Length);
                        bytes = memoryStream.ToArray();
                    }
                }
                byte[] array2 = encryption.Encrypt(bytes);
                writer.Write(array2, 0, array2.Length);
            }
            public void Read(Stream stream, BinaryReader reader, DataAesEncryption encryption)
            {
                int cipherSize = encryption.GetCipherSize(288);
                byte[] bytes = reader.ReadBytes(cipherSize);
                byte[] buffer = null;
                try
                {
                    buffer = encryption.Decrypt(bytes);
                }
                catch (Exception message)
                {
                    throw new Exception("DataManager::MetaData::Read: " + message.Message);
                }
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                    {
                        char[] array = binaryReader.ReadChars(4);
                        if (array[0] != 'S' && array[1] != 'A' && array[2] != 'V' && array[3] != 'E')
                        {
                            throw new Exception("DataManager::MetaData::Read: DataCorruption. Expected \"SAVE\" as header.");
                        }
                        else
                        {
                            this.SaveVersion = binaryReader.ReadSingle();
                            this.DataSize = binaryReader.ReadInt32();
                            this.LatestSlot = binaryReader.ReadInt32();
                            this.LatestSave = binaryReader.ReadInt32();
                            this.LatestTimestamp = binaryReader.ReadDouble();
                            this.IsGameFinishFlag = binaryReader.ReadInt32();
                            this.SelectedLanguage = binaryReader.ReadInt32();
                            this.IsAutoLogin = binaryReader.ReadSByte();
                            int num = binaryReader.Read(this.SystemAchievementStatuses, 0, this.SystemAchievementStatuses.Length);
                            this.ScreenRotation = binaryReader.ReadByte();
                        }
                    }
                }
            }
        }
        protected DataManager.MetaData metaData = new DataManager.MetaData();
        DataAesEncryption Encryption = new DataAesEncryption();
        const int BLOCK_SIZE = 18432;
        

        public void Load(int slotID, int saveID, string file, out byte[] result) //slot max = 9 save max = 14
        {
            try
            {
                result = null;
                using (FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        this.metaData.Read(fileStream, binaryReader, this.Encryption);
                        fileStream.Seek((long)(320 - (int)fileStream.Position), SeekOrigin.Current);
                        long num = 153600;
                        fileStream.Seek(num, SeekOrigin.Current);
                        long num2 = BLOCK_SIZE + slotID * 15 * BLOCK_SIZE + saveID * BLOCK_SIZE;

                        fileStream.Seek(num2, SeekOrigin.Current);
                        int cipherSize = this.Encryption.GetCipherSize(this.metaData.DataSize + 4);
                        byte[] array = new byte[cipherSize];
                        binaryReader.Read(array, 0, array.Length);
                        byte[] array2 = null;
                        try
                        {
                            array2 = this.Encryption.Decrypt(array);
                            result = array2;
                        }
                        catch (Exception ex)
                        {
                            result = null;
                            throw new Exception("DataCorruption noticed at decryption attempt. " + ex.Message);
                        }
                        if (array2.Length != this.metaData.DataSize + 4)
                        {
                            result = null;
                            throw new Exception("PlainText.size and metaData.DataSize is NOT equal.");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                result = null;
                throw new Exception("DataManager::Load: " + exc.Message);
            }
        }
        
        public void Save(int slotID, int saveID, string file, byte[] decryptedFile, out bool success)
        {
            success = false;
            try
            {
                using (FileStream fileStream = File.Open(/*SharedDataBytesStorage.MetaData.FilePath*/ file, FileMode.Open, FileAccess.ReadWrite))
                {
                    //using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    //{
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        //this.metaData.WriteLatestSlotAndSaveAndLatestTimestamp(fileStream, binaryWriter, binaryReader, this.Encryption);
                        fileStream.Seek((long)(320 - (int)fileStream.Position), SeekOrigin.Current);
                        long num2 = 153600;
                        //ISharedDataLog.Log("Seek to: " + num2);
                        fileStream.Seek(num2, SeekOrigin.Current);
                        /*byte[] bytes2 = null;
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream))
                            {
                                this.WriteDataToStream(list, this.dataTypeList, memoryStream, binaryWriter2);
                                bytes2 = memoryStream.ToArray();
                            }
                        }*/
                        byte[] encryptedFile = this.Encryption.Encrypt(decryptedFile); //var bytes2
                                                                                       //if (!isAutosave)
                                                                                       //{
                        long num3 = BLOCK_SIZE + slotID * 15 * BLOCK_SIZE + saveID * BLOCK_SIZE;
                        //ISharedDataLog.Log("Seek to: " + num3);
                        fileStream.Seek(num3, SeekOrigin.Current);
                        //}
                        binaryWriter.Write(encryptedFile, 0, encryptedFile.Length);
                    }
                    //}
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                throw new Exception("DataManager::Save: " + ex.Message);
            }
        }
    }
}
