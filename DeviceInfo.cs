//
// C# (WinAPI)
// https://github.com/dkxce/DeviceInfo
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace dkxce
{
    /// <summary>
    ///     Device Information (WinAPI)
    /// </summary>
    public sealed class DeviceInfo : IDisposable
    {
        #region WinAPI   

        #region DLL Calls

        #region setupapi.dll
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiChangeState(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevsW([In] ref Guid ClassGuid, [MarshalAs(UnmanagedType.LPWStr)]string Enumerator, IntPtr parent, UInt32 flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDevicePropertyW(IntPtr deviceInfoSet,[In] ref SP_DEVINFO_DATA DeviceInfoData,[In] ref DEVPROPKEY propertyKey,[Out] out UInt32 propertyType,IntPtr propertyBuffer,UInt32 propertyBufferSize,out UInt32 requiredSize,UInt32 flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryPropertyW(IntPtr DeviceInfoSet, [In] ref SP_DEVINFO_DATA DeviceInfoData, UInt32 Property, [Out] out UInt32 PropertyRegDataType, IntPtr PropertyBuffer, UInt32 PropertyBufferSize, [In, Out] ref UInt32 RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, string Enumerator, IntPtr hwndParent, DIGCF Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs([In] ref Guid classGuid, string enumerator, IntPtr hwndParent, DIGCF Flags);


        [DllImport("setupapi.dll")]
        private static extern int CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

        [DllImport("setupapi.dll")]
        private static extern int CM_Get_Device_ID(uint dnDevInst, IntPtr Buffer, int BufferLen, uint ulFlags);

        [DllImport("setupapi.dll")]
        private static extern int CM_Get_Child(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

        [DllImport("setupapi.dll")]
        private static extern int CM_Get_Sibling(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

        [DllImport("setupapi.dll")]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true, EntryPoint = "SetupDiGetDevicePropertyW")]
        private static extern bool SetupDiGetDeviceProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref DEVPROPKEY propertyKey, out int propertyType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize, int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, ref SP_DEVINFO_DATA devInfo, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, ref UInt32 requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, ref UInt32 requiredSize, IntPtr deviceInfoData);        

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData, [In] ref SP_PROPCHANGE_PARAMS classInstallParams, uint ClassInstallParamsSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData, [In] ref PropertyChangeParameters classInstallParams, int classInstallParamsSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiCallClassInstaller(DiFunction installFunction, IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceInstanceId(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA did, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder DeviceInstanceId, int DeviceInstanceIdSize, out int RequiredSize);
        #endregion setupapi.dll

        #region kernel32.dll
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.U4)] FileAccess access, [MarshalAs(UnmanagedType.U4)] FileShare share, /* optional SECURITY_ATTRIBUTES struct or IntPtr.Zero */ IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, ref STORAGE_DEVICE_NUMBER lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetVolumeNameForVolumeMountPointW")]
        private static extern bool GetVolumeNameForVolumeMountPoint(string lpszVolumeMountPoint, [Out] StringBuilder lpszVolumeName, uint cchBufferLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetVolumePathNamesForVolumeNameW([MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeName, [MarshalAs(UnmanagedType.LPWStr)] string lpszVolumePathNames, uint cchBuferLength, ref UInt32 lpcchReturnLength);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetVolumePathNamesForVolumeName(string lpszVolumeName, char[] lpszVolumePathNames, uint cchBuferLength, out UInt32 lpcchReturnLength);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);
        #endregion kernel32.dll

        #region cfgmgr32.dll
        /// <summary>
        ///     Get Device Detailed Status (CM_Get_DevNode_Status)
        /// </summary>
        /// <param name="status">Device Detailed Status Code</param>
        /// <param name="probNum">Device Detailed Problem Code</param>
        /// <param name="devInst">Device Pointer</param>
        /// <param name="flags">Operation Flags</param>
        /// <returns>Complete Status</returns>
        [DllImport("cfgmgr32.dll", SetLastError = true)]
        private static extern CR_STATUS CM_Get_DevNode_Status(out DN_STATUS status, out DN_PROBLEM probNum, IntPtr devInst, int flags);

        /// <summary>
        ///     Get Device Pointer for DeviceID for Detailed Status)
        /// </summary>
        /// <param name="pdnDevInst">Device Pointer</param>
        /// <param name="pDeviceID">DeviceID</param>
        /// <param name="ulFlags">Operation Flags</param>
        /// <returns>Complete Status</returns>
        [DllImport("cfgmgr32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int CM_Locate_DevNodeW(ref IntPtr pdnDevInst, string pDeviceID, ulong ulFlags);
        #endregion cfgmgr32.dll

        #region shell32.dll

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);


        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, IntPtr phiconSmall, int nIcons);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);

        #endregion shell32.dll

        #region user32.dll

        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        private static extern int DestroyIcon(IntPtr hIcon);

        #endregion

        #endregion DLL Calls

        #region Enums & Structs

        #region Flags

        [Flags]
        private enum DIGCF : uint
        {
            DIGCF_DEFAULT = 0x00000001,
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010,
        }

        [Flags]
        private enum EMethod : uint
        {
            Buffered = 0,
            InDirect = 1,
            OutDirect = 2,
            Neither = 3
        }

        [Flags]
        private enum EFileDevice : uint
        {
            Beep = 0x00000001,
            CDRom = 0x00000002,
            CDRomFileSytem = 0x00000003,
            Controller = 0x00000004,
            Datalink = 0x00000005,
            Dfs = 0x00000006,
            Disk = 0x00000007,
            DiskFileSystem = 0x00000008,
            FileSystem = 0x00000009,
            InPortPort = 0x0000000a,
            Keyboard = 0x0000000b,
            Mailslot = 0x0000000c,
            MidiIn = 0x0000000d,
            MidiOut = 0x0000000e,
            Mouse = 0x0000000f,
            MultiUncProvider = 0x00000010,
            NamedPipe = 0x00000011,
            Network = 0x00000012,
            NetworkBrowser = 0x00000013,
            NetworkFileSystem = 0x00000014,
            Null = 0x00000015,
            ParallelPort = 0x00000016,
            PhysicalNetcard = 0x00000017,
            Printer = 0x00000018,
            Scanner = 0x00000019,
            SerialMousePort = 0x0000001a,
            SerialPort = 0x0000001b,
            Screen = 0x0000001c,
            Sound = 0x0000001d,
            Streams = 0x0000001e,
            Tape = 0x0000001f,
            TapeFileSystem = 0x00000020,
            Transport = 0x00000021,
            Unknown = 0x00000022,
            Video = 0x00000023,
            VirtualDisk = 0x00000024,
            WaveIn = 0x00000025,
            WaveOut = 0x00000026,
            Port8042 = 0x00000027,
            NetworkRedirector = 0x00000028,
            Battery = 0x00000029,
            BusExtender = 0x0000002a,
            Modem = 0x0000002b,
            Vdm = 0x0000002c,
            MassStorage = 0x0000002d,
            Smb = 0x0000002e,
            Ks = 0x0000002f,
            Changer = 0x00000030,
            Smartcard = 0x00000031,
            Acpi = 0x00000032,
            Dvd = 0x00000033,
            FullscreenVideo = 0x00000034,
            DfsFileSystem = 0x00000035,
            DfsVolume = 0x00000036,
            Serenum = 0x00000037,
            Termsrv = 0x00000038,
            Ksec = 0x00000039,
            // From Windows Driver Kit 7
            Fips = 0x0000003A,
            Infiniband = 0x0000003B,
            Vmbus = 0x0000003E,
            CryptProvider = 0x0000003F,
            Wpd = 0x00000040,
            Bluetooth = 0x00000041,
            MtComposite = 0x00000042,
            MtTransport = 0x00000043,
            Biometric = 0x00000044,
            Pmi = 0x00000045
        }

        [Flags]
        private enum EIOControlCode : uint
        {
            // STORAGE
            StorageCheckVerify = (EFileDevice.MassStorage << 16) | (0x0200 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageCheckVerify2 = (EFileDevice.MassStorage << 16) | (0x0200 << 2) | EMethod.Buffered | (0 << 14), // FileAccess.Any
            StorageMediaRemoval = (EFileDevice.MassStorage << 16) | (0x0201 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageEjectMedia = (EFileDevice.MassStorage << 16) | (0x0202 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageLoadMedia = (EFileDevice.MassStorage << 16) | (0x0203 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageLoadMedia2 = (EFileDevice.MassStorage << 16) | (0x0203 << 2) | EMethod.Buffered | (0 << 14),
            StorageReserve = (EFileDevice.MassStorage << 16) | (0x0204 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageRelease = (EFileDevice.MassStorage << 16) | (0x0205 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageFindNewDevices = (EFileDevice.MassStorage << 16) | (0x0206 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageEjectionControl = (EFileDevice.MassStorage << 16) | (0x0250 << 2) | EMethod.Buffered | (0 << 14),
            StorageMcnControl = (EFileDevice.MassStorage << 16) | (0x0251 << 2) | EMethod.Buffered | (0 << 14),
            StorageGetMediaTypes = (EFileDevice.MassStorage << 16) | (0x0300 << 2) | EMethod.Buffered | (0 << 14),
            StorageGetMediaTypesEx = (EFileDevice.MassStorage << 16) | (0x0301 << 2) | EMethod.Buffered | (0 << 14),
            StorageResetBus = (EFileDevice.MassStorage << 16) | (0x0400 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageResetDevice = (EFileDevice.MassStorage << 16) | (0x0401 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            StorageGetDeviceNumber = (EFileDevice.MassStorage << 16) | (0x0420 << 2) | EMethod.Buffered | (0 << 14),
            StoragePredictFailure = (EFileDevice.MassStorage << 16) | (0x0440 << 2) | EMethod.Buffered | (0 << 14),
            StorageObsoleteResetBus = (EFileDevice.MassStorage << 16) | (0x0400 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            StorageObsoleteResetDevice = (EFileDevice.MassStorage << 16) | (0x0401 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            StorageQueryProperty = (EFileDevice.MassStorage << 16) | (0x0500 << 2) | EMethod.Buffered | (0 << 14),
            // DISK
            DiskGetDriveGeometry = (EFileDevice.Disk << 16) | (0x0000 << 2) | EMethod.Buffered | (0 << 14),
            DiskGetDriveGeometryEx = (EFileDevice.Disk << 16) | (0x0028 << 2) | EMethod.Buffered | (0 << 14),
            DiskGetPartitionInfo = (EFileDevice.Disk << 16) | (0x0001 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskGetPartitionInfoEx = (EFileDevice.Disk << 16) | (0x0012 << 2) | EMethod.Buffered | (0 << 14),
            DiskSetPartitionInfo = (EFileDevice.Disk << 16) | (0x0002 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetDriveLayout = (EFileDevice.Disk << 16) | (0x0003 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskSetDriveLayout = (EFileDevice.Disk << 16) | (0x0004 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskVerify = (EFileDevice.Disk << 16) | (0x0005 << 2) | EMethod.Buffered | (0 << 14),
            DiskFormatTracks = (EFileDevice.Disk << 16) | (0x0006 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskReassignBlocks = (EFileDevice.Disk << 16) | (0x0007 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskPerformance = (EFileDevice.Disk << 16) | (0x0008 << 2) | EMethod.Buffered | (0 << 14),
            DiskIsWritable = (EFileDevice.Disk << 16) | (0x0009 << 2) | EMethod.Buffered | (0 << 14),
            DiskLogging = (EFileDevice.Disk << 16) | (0x000a << 2) | EMethod.Buffered | (0 << 14),
            DiskFormatTracksEx = (EFileDevice.Disk << 16) | (0x000b << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskHistogramStructure = (EFileDevice.Disk << 16) | (0x000c << 2) | EMethod.Buffered | (0 << 14),
            DiskHistogramData = (EFileDevice.Disk << 16) | (0x000d << 2) | EMethod.Buffered | (0 << 14),
            DiskHistogramReset = (EFileDevice.Disk << 16) | (0x000e << 2) | EMethod.Buffered | (0 << 14),
            DiskRequestStructure = (EFileDevice.Disk << 16) | (0x000f << 2) | EMethod.Buffered | (0 << 14),
            DiskRequestData = (EFileDevice.Disk << 16) | (0x0010 << 2) | EMethod.Buffered | (0 << 14),
            DiskControllerNumber = (EFileDevice.Disk << 16) | (0x0011 << 2) | EMethod.Buffered | (0 << 14),
            DiskSmartGetVersion = (EFileDevice.Disk << 16) | (0x0020 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskSmartSendDriveCommand = (EFileDevice.Disk << 16) | (0x0021 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskSmartRcvDriveData = (EFileDevice.Disk << 16) | (0x0022 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskUpdateDriveSize = (EFileDevice.Disk << 16) | (0x0032 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGrowPartition = (EFileDevice.Disk << 16) | (0x0034 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetCacheInformation = (EFileDevice.Disk << 16) | (0x0035 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskSetCacheInformation = (EFileDevice.Disk << 16) | (0x0036 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskDeleteDriveLayout = (EFileDevice.Disk << 16) | (0x0040 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskFormatDrive = (EFileDevice.Disk << 16) | (0x00f3 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskSenseDevice = (EFileDevice.Disk << 16) | (0x00f8 << 2) | EMethod.Buffered | (0 << 14),
            DiskCheckVerify = (EFileDevice.Disk << 16) | (0x0200 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskMediaRemoval = (EFileDevice.Disk << 16) | (0x0201 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskEjectMedia = (EFileDevice.Disk << 16) | (0x0202 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskLoadMedia = (EFileDevice.Disk << 16) | (0x0203 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskReserve = (EFileDevice.Disk << 16) | (0x0204 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskRelease = (EFileDevice.Disk << 16) | (0x0205 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskFindNewDevices = (EFileDevice.Disk << 16) | (0x0206 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DiskGetMediaTypes = (EFileDevice.Disk << 16) | (0x0300 << 2) | EMethod.Buffered | (0 << 14),
            DiskSetPartitionInfoEx = (EFileDevice.Disk << 16) | (0x0013 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetDriveLayoutEx = (EFileDevice.Disk << 16) | (0x0014 << 2) | EMethod.Buffered | (0 << 14),
            DiskSetDriveLayoutEx = (EFileDevice.Disk << 16) | (0x0015 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskCreateDisk = (EFileDevice.Disk << 16) | (0x0016 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            DiskGetLengthInfo = (EFileDevice.Disk << 16) | (0x0017 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            // CHANGER
            ChangerGetParameters = (EFileDevice.Changer << 16) | (0x0000 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerGetStatus = (EFileDevice.Changer << 16) | (0x0001 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerGetProductData = (EFileDevice.Changer << 16) | (0x0002 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerSetAccess = (EFileDevice.Changer << 16) | (0x0004 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            ChangerGetElementStatus = (EFileDevice.Changer << 16) | (0x0005 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            ChangerInitializeElementStatus = (EFileDevice.Changer << 16) | (0x0006 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerSetPosition = (EFileDevice.Changer << 16) | (0x0007 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerExchangeMedium = (EFileDevice.Changer << 16) | (0x0008 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerMoveMedium = (EFileDevice.Changer << 16) | (0x0009 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerReinitializeTarget = (EFileDevice.Changer << 16) | (0x000A << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            ChangerQueryVolumeTags = (EFileDevice.Changer << 16) | (0x000B << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            // FILESYSTEM
            FsctlRequestOplockLevel1 = (EFileDevice.FileSystem << 16) | (0 << 2) | EMethod.Buffered | (0 << 14),
            FsctlRequestOplockLevel2 = (EFileDevice.FileSystem << 16) | (1 << 2) | EMethod.Buffered | (0 << 14),
            FsctlRequestBatchOplock = (EFileDevice.FileSystem << 16) | (2 << 2) | EMethod.Buffered | (0 << 14),
            FsctlOplockBreakAcknowledge = (EFileDevice.FileSystem << 16) | (3 << 2) | EMethod.Buffered | (0 << 14),
            FsctlOpBatchAckClosePending = (EFileDevice.FileSystem << 16) | (4 << 2) | EMethod.Buffered | (0 << 14),
            FsctlOplockBreakNotify = (EFileDevice.FileSystem << 16) | (5 << 2) | EMethod.Buffered | (0 << 14),
            FsctlLockVolume = (EFileDevice.FileSystem << 16) | (6 << 2) | EMethod.Buffered | (0 << 14),
            FsctlUnlockVolume = (EFileDevice.FileSystem << 16) | (7 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDismountVolume = (EFileDevice.FileSystem << 16) | (8 << 2) | EMethod.Buffered | (0 << 14),
            FsctlIsVolumeMounted = (EFileDevice.FileSystem << 16) | (10 << 2) | EMethod.Buffered | (0 << 14),
            FsctlIsPathnameValid = (EFileDevice.FileSystem << 16) | (11 << 2) | EMethod.Buffered | (0 << 14),
            FsctlMarkVolumeDirty = (EFileDevice.FileSystem << 16) | (12 << 2) | EMethod.Buffered | (0 << 14),
            FsctlQueryRetrievalPointers = (EFileDevice.FileSystem << 16) | (14 << 2) | EMethod.Neither | (0 << 14),
            FsctlGetCompression = (EFileDevice.FileSystem << 16) | (15 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetCompression = (EFileDevice.FileSystem << 16) | (16 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            FsctlMarkAsSystemHive = (EFileDevice.FileSystem << 16) | (19 << 2) | EMethod.Neither | (0 << 14),
            FsctlOplockBreakAckNo2 = (EFileDevice.FileSystem << 16) | (20 << 2) | EMethod.Buffered | (0 << 14),
            FsctlInvalidateVolumes = (EFileDevice.FileSystem << 16) | (21 << 2) | EMethod.Buffered | (0 << 14),
            FsctlQueryFatBpb = (EFileDevice.FileSystem << 16) | (22 << 2) | EMethod.Buffered | (0 << 14),
            FsctlRequestFilterOplock = (EFileDevice.FileSystem << 16) | (23 << 2) | EMethod.Buffered | (0 << 14),
            FsctlFileSystemGetStatistics = (EFileDevice.FileSystem << 16) | (24 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetNtfsVolumeData = (EFileDevice.FileSystem << 16) | (25 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetNtfsFileRecord = (EFileDevice.FileSystem << 16) | (26 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetVolumeBitmap = (EFileDevice.FileSystem << 16) | (27 << 2) | EMethod.Neither | (0 << 14),
            FsctlGetRetrievalPointers = (EFileDevice.FileSystem << 16) | (28 << 2) | EMethod.Neither | (0 << 14),
            FsctlMoveFile = (EFileDevice.FileSystem << 16) | (29 << 2) | EMethod.Buffered | (0 << 14),
            FsctlIsVolumeDirty = (EFileDevice.FileSystem << 16) | (30 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetHfsInformation = (EFileDevice.FileSystem << 16) | (31 << 2) | EMethod.Buffered | (0 << 14),
            FsctlAllowExtendedDasdIo = (EFileDevice.FileSystem << 16) | (32 << 2) | EMethod.Neither | (0 << 14),
            FsctlReadPropertyData = (EFileDevice.FileSystem << 16) | (33 << 2) | EMethod.Neither | (0 << 14),
            FsctlWritePropertyData = (EFileDevice.FileSystem << 16) | (34 << 2) | EMethod.Neither | (0 << 14),
            FsctlFindFilesBySid = (EFileDevice.FileSystem << 16) | (35 << 2) | EMethod.Neither | (0 << 14),
            FsctlDumpPropertyData = (EFileDevice.FileSystem << 16) | (37 << 2) | EMethod.Neither | (0 << 14),
            FsctlSetObjectId = (EFileDevice.FileSystem << 16) | (38 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetObjectId = (EFileDevice.FileSystem << 16) | (39 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDeleteObjectId = (EFileDevice.FileSystem << 16) | (40 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetReparsePoint = (EFileDevice.FileSystem << 16) | (41 << 2) | EMethod.Buffered | (0 << 14),
            FsctlGetReparsePoint = (EFileDevice.FileSystem << 16) | (42 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDeleteReparsePoint = (EFileDevice.FileSystem << 16) | (43 << 2) | EMethod.Buffered | (0 << 14),
            FsctlEnumUsnData = (EFileDevice.FileSystem << 16) | (44 << 2) | EMethod.Neither | (0 << 14),
            FsctlSecurityIdCheck = (EFileDevice.FileSystem << 16) | (45 << 2) | EMethod.Neither | (FileAccess.Read << 14),
            FsctlReadUsnJournal = (EFileDevice.FileSystem << 16) | (46 << 2) | EMethod.Neither | (0 << 14),
            FsctlSetObjectIdExtended = (EFileDevice.FileSystem << 16) | (47 << 2) | EMethod.Buffered | (0 << 14),
            FsctlCreateOrGetObjectId = (EFileDevice.FileSystem << 16) | (48 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetSparse = (EFileDevice.FileSystem << 16) | (49 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSetZeroData = (EFileDevice.FileSystem << 16) | (50 << 2) | EMethod.Buffered | (FileAccess.Write << 14),
            FsctlQueryAllocatedRanges = (EFileDevice.FileSystem << 16) | (51 << 2) | EMethod.Neither | (FileAccess.Read << 14),
            FsctlEnableUpgrade = (EFileDevice.FileSystem << 16) | (52 << 2) | EMethod.Buffered | (FileAccess.Write << 14),
            FsctlSetEncryption = (EFileDevice.FileSystem << 16) | (53 << 2) | EMethod.Neither | (0 << 14),
            FsctlEncryptionFsctlIo = (EFileDevice.FileSystem << 16) | (54 << 2) | EMethod.Neither | (0 << 14),
            FsctlWriteRawEncrypted = (EFileDevice.FileSystem << 16) | (55 << 2) | EMethod.Neither | (0 << 14),
            FsctlReadRawEncrypted = (EFileDevice.FileSystem << 16) | (56 << 2) | EMethod.Neither | (0 << 14),
            FsctlCreateUsnJournal = (EFileDevice.FileSystem << 16) | (57 << 2) | EMethod.Neither | (0 << 14),
            FsctlReadFileUsnData = (EFileDevice.FileSystem << 16) | (58 << 2) | EMethod.Neither | (0 << 14),
            FsctlWriteUsnCloseRecord = (EFileDevice.FileSystem << 16) | (59 << 2) | EMethod.Neither | (0 << 14),
            FsctlExtendVolume = (EFileDevice.FileSystem << 16) | (60 << 2) | EMethod.Buffered | (0 << 14),
            FsctlQueryUsnJournal = (EFileDevice.FileSystem << 16) | (61 << 2) | EMethod.Buffered | (0 << 14),
            FsctlDeleteUsnJournal = (EFileDevice.FileSystem << 16) | (62 << 2) | EMethod.Buffered | (0 << 14),
            FsctlMarkHandle = (EFileDevice.FileSystem << 16) | (63 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSisCopyFile = (EFileDevice.FileSystem << 16) | (64 << 2) | EMethod.Buffered | (0 << 14),
            FsctlSisLinkFiles = (EFileDevice.FileSystem << 16) | (65 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            FsctlHsmMsg = (EFileDevice.FileSystem << 16) | (66 << 2) | EMethod.Buffered | (FileAccess.ReadWrite << 14),
            FsctlNssControl = (EFileDevice.FileSystem << 16) | (67 << 2) | EMethod.Buffered | (FileAccess.Write << 14),
            FsctlHsmData = (EFileDevice.FileSystem << 16) | (68 << 2) | EMethod.Neither | (FileAccess.ReadWrite << 14),
            FsctlRecallFile = (EFileDevice.FileSystem << 16) | (69 << 2) | EMethod.Neither | (0 << 14),
            FsctlNssRcontrol = (EFileDevice.FileSystem << 16) | (70 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            // VIDEO
            VideoQuerySupportedBrightness = (EFileDevice.Video << 16) | (0x0125 << 2) | EMethod.Buffered | (0 << 14),
            VideoQueryDisplayBrightness = (EFileDevice.Video << 16) | (0x0126 << 2) | EMethod.Buffered | (0 << 14),
            VideoSetDisplayBrightness = (EFileDevice.Video << 16) | (0x0127 << 2) | EMethod.Buffered | (0 << 14)
        }

        [Flags()]
        private enum Scopes
        {
            Global = 1,
            ConfigSpecific = 2,
            ConfigGeneral = 4
        }

        /// <summary>
        ///     Device Detailed Statuses
        /// </summary>
        [Flags]
        public enum DN_STATUS : uint
        {
            UNKNOWN = 0,
            DN_ROOT_ENUMERATED = (0x00000001), // Was enumerated by ROOT
            DN_DRIVER_LOADED = (0x00000002), // Has Register_Device_Driver
            DN_ENUM_LOADED = (0x00000004), // Has Register_Enumerator
            DN_STARTED = (0x00000008), // Is currently configured
            DN_MANUAL = (0x00000010), // Manually installed
            DN_NEED_TO_ENUM = (0x00000020), // May need reenumeration
            DN_NOT_FIRST_TIME = (0x00000040), // Has received a config
            DN_HARDWARE_ENUM = (0x00000080), // Enum generates hardware ID
            DN_LIAR = (0x00000100), // Lied about can reconfig once
            DN_HAS_MARK = (0x00000200), // Not CM_Create_DevInst lately
            DN_HAS_PROBLEM = (0x00000400), // Need device installer
            DN_FILTERED = (0x00000800), // Is filtered
            DN_MOVED = (0x00001000), // Has been moved
            DN_DISABLEABLE = (0x00002000), // Can be disabled
            DN_REMOVABLE = (0x00004000), // Can be removed
            DN_PRIVATE_PROBLEM = (0x00008000), // Has a private problem
            DN_MF_PARENT = (0x00010000), // Multi function parent
            DN_MF_CHILD = (0x00020000), // Multi function child
            DN_WILL_BE_REMOVED = (0x00040000) // DevInst is being removed
        }

        #endregion Flags

        #region Enums

        private enum DiFunction
        {
            SelectDevice = 1,
            InstallDevice = 2,
            AssignResources = 3,
            Properties = 4,
            Remove = 5,
            FirstTimeSetup = 6,
            FoundDevice = 7,
            SelectClassDrivers = 8,
            ValidateClassDrivers = 9,
            InstallClassDrivers = (int)0xa,
            CalcDiskSpace = (int)0xb,
            DestroyPrivateData = (int)0xc,
            ValidateDriver = (int)0xd,
            Detect = (int)0xf,
            InstallWizard = (int)0x10,
            DestroyWizardData = (int)0x11,
            PropertyChange = (int)0x12,
            EnableClass = (int)0x13,
            DetectVerify = (int)0x14,
            InstallDeviceFiles = (int)0x15,
            UnRemove = (int)0x16,
            SelectBestCompatDrv = (int)0x17,
            AllowInstall = (int)0x18,
            RegisterDevice = (int)0x19,
            NewDeviceWizardPreSelect = (int)0x1a,
            NewDeviceWizardSelect = (int)0x1b,
            NewDeviceWizardPreAnalyze = (int)0x1c,
            NewDeviceWizardPostAnalyze = (int)0x1d,
            NewDeviceWizardFinishInstall = (int)0x1e,
            Unused1 = (int)0x1f,
            InstallInterfaces = (int)0x20,
            DetectCancel = (int)0x21,
            RegisterCoInstallers = (int)0x22,
            AddPropertyPageAdvanced = (int)0x23,
            AddPropertyPageBasic = (int)0x24,
            Reserved1 = (int)0x25,
            Troubleshooter = (int)0x26,
            PowerMessageWake = (int)0x27,
            AddRemotePropertyPageAdvanced = (int)0x28,
            UpdateDriverUI = (int)0x29,
            Reserved2 = (int)0x30
        }

        private enum StateChangeAction
        {
            Enable = 1,
            Disable = 2,
            PropChange = 3,
            Start = 4,
            Stop = 5
        }        

        private enum SetupDiGetDeviceRegistryPropertyEnum : uint
        {
            SPDRP_DEVICEDESC = 0x00000000, // DeviceDesc (R/W)
            SPDRP_HARDWAREID = 0x00000001, // HardwareID (R/W)
            SPDRP_COMPATIBLEIDS = 0x00000002, // CompatibleIDs (R/W)
            SPDRP_UNUSED0 = 0x00000003, // unused
            SPDRP_SERVICE = 0x00000004, // Service (R/W)
            SPDRP_UNUSED1 = 0x00000005, // unused
            SPDRP_UNUSED2 = 0x00000006, // unused
            SPDRP_CLASS = 0x00000007, // Class (R--tied to ClassGUID)
            SPDRP_CLASSGUID = 0x00000008, // ClassGUID (R/W)
            SPDRP_DRIVER = 0x00000009, // Driver (R/W)
            SPDRP_CONFIGFLAGS = 0x0000000A, // ConfigFlags (R/W)
            SPDRP_MFG = 0x0000000B, // Mfg (R/W)
            SPDRP_FRIENDLYNAME = 0x0000000C, // FriendlyName (R/W)
            SPDRP_LOCATION_INFORMATION = 0x0000000D, // LocationInformation (R/W)
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E, // PhysicalDeviceObjectName (R)
            SPDRP_CAPABILITIES = 0x0000000F, // Capabilities (R)
            SPDRP_UI_NUMBER = 0x00000010, // UiNumber (R)
            SPDRP_UPPERFILTERS = 0x00000011, // UpperFilters (R/W)
            SPDRP_LOWERFILTERS = 0x00000012, // LowerFilters (R/W)
            SPDRP_BUSTYPEGUID = 0x00000013, // BusTypeGUID (R)
            SPDRP_LEGACYBUSTYPE = 0x00000014, // LegacyBusType (R)
            SPDRP_BUSNUMBER = 0x00000015, // BusNumber (R)
            SPDRP_ENUMERATOR_NAME = 0x00000016, // Enumerator Name (R)
            SPDRP_SECURITY = 0x00000017, // Security (R/W, binary form)
            SPDRP_SECURITY_SDS = 0x00000018, // Security (W, SDS form)
            SPDRP_DEVTYPE = 0x00000019, // Device Type (R/W)
            SPDRP_EXCLUSIVE = 0x0000001A, // Device is exclusive-access (R/W)
            SPDRP_CHARACTERISTICS = 0x0000001B, // Device Characteristics (R/W)
            SPDRP_ADDRESS = 0x0000001C, // Device Address (R)
            SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D, // UiNumberDescFormat (R/W)
            SPDRP_DEVICE_POWER_DATA = 0x0000001E, // Device Power Data (R)
            SPDRP_REMOVAL_POLICY = 0x0000001F, // Removal Policy (R)
            SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020, // Hardware Removal Policy (R)
            SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021, // Removal Policy Override (RW)
            SPDRP_INSTALL_STATE = 0x00000022, // Device Install State (R)
            SPDRP_LOCATION_PATHS = 0x00000023, // Device Location Paths (R)
            SPDRP_BASE_CONTAINERID = 0x00000024  // Base ContainerID (R)
        }

        private enum SetupApiError
        {
            NoAssociatedClass = unchecked((int)0xe0000200),
            ClassMismatch = unchecked((int)0xe0000201),
            DuplicateFound = unchecked((int)0xe0000202),
            NoDriverSelected = unchecked((int)0xe0000203),
            KeyDoesNotExist = unchecked((int)0xe0000204),
            InvalidDevinstName = unchecked((int)0xe0000205),
            InvalidClass = unchecked((int)0xe0000206),
            DevinstAlreadyExists = unchecked((int)0xe0000207),
            DevinfoNotRegistered = unchecked((int)0xe0000208),
            InvalidRegProperty = unchecked((int)0xe0000209),
            NoInf = unchecked((int)0xe000020a),
            NoSuchHDevinst = unchecked((int)0xe000020b),
            CantLoadClassIcon = unchecked((int)0xe000020c),
            InvalidClassInstaller = unchecked((int)0xe000020d),
            DiDoDefault = unchecked((int)0xe000020e),
            DiNoFileCopy = unchecked((int)0xe000020f),
            InvalidHwProfile = unchecked((int)0xe0000210),
            NoDeviceSelected = unchecked((int)0xe0000211),
            DevinfolistLocked = unchecked((int)0xe0000212),
            DevinfodataLocked = unchecked((int)0xe0000213),
            DiBadPath = unchecked((int)0xe0000214),
            NoClassInstallParams = unchecked((int)0xe0000215),
            FileQueueLocked = unchecked((int)0xe0000216),
            BadServiceInstallSect = unchecked((int)0xe0000217),
            NoClassDriverList = unchecked((int)0xe0000218),
            NoAssociatedService = unchecked((int)0xe0000219),
            NoDefaultDeviceInterface = unchecked((int)0xe000021a),
            DeviceInterfaceActive = unchecked((int)0xe000021b),
            DeviceInterfaceRemoved = unchecked((int)0xe000021c),
            BadInterfaceInstallSect = unchecked((int)0xe000021d),
            NoSuchInterfaceClass = unchecked((int)0xe000021e),
            InvalidReferenceString = unchecked((int)0xe000021f),
            InvalidMachineName = unchecked((int)0xe0000220),
            RemoteCommFailure = unchecked((int)0xe0000221),
            MachineUnavailable = unchecked((int)0xe0000222),
            NoConfigMgrServices = unchecked((int)0xe0000223),
            InvalidPropPageProvider = unchecked((int)0xe0000224),
            NoSuchDeviceInterface = unchecked((int)0xe0000225),
            DiPostProcessingRequired = unchecked((int)0xe0000226),
            InvalidCOInstaller = unchecked((int)0xe0000227),
            NoCompatDrivers = unchecked((int)0xe0000228),
            NoDeviceIcon = unchecked((int)0xe0000229),
            InvalidInfLogConfig = unchecked((int)0xe000022a),
            DiDontInstall = unchecked((int)0xe000022b),
            InvalidFilterDriver = unchecked((int)0xe000022c),
            NonWindowsNTDriver = unchecked((int)0xe000022d),
            NonWindowsDriver = unchecked((int)0xe000022e),
            NoCatalogForOemInf = unchecked((int)0xe000022f),
            DevInstallQueueNonNative = unchecked((int)0xe0000230),
            NotDisableable = unchecked((int)0xe0000231),
            CantRemoveDevinst = unchecked((int)0xe0000232),
            InvalidTarget = unchecked((int)0xe0000233),
            DriverNonNative = unchecked((int)0xe0000234),
            InWow64 = unchecked((int)0xe0000235),
            SetSystemRestorePoint = unchecked((int)0xe0000236),
            IncorrectlyCopiedInf = unchecked((int)0xe0000237),
            SceDisabled = unchecked((int)0xe0000238),
            UnknownException = unchecked((int)0xe0000239),
            PnpRegistryError = unchecked((int)0xe000023a),
            RemoteRequestUnsupported = unchecked((int)0xe000023b),
            NotAnInstalledOemInf = unchecked((int)0xe000023c),
            InfInUseByDevices = unchecked((int)0xe000023d),
            DiFunctionObsolete = unchecked((int)0xe000023e),
            NoAuthenticodeCatalog = unchecked((int)0xe000023f),
            AuthenticodeDisallowed = unchecked((int)0xe0000240),
            AuthenticodeTrustedPublisher = unchecked((int)0xe0000241),
            AuthenticodeTrustNotEstablished = unchecked((int)0xe0000242),
            AuthenticodePublisherNotTrusted = unchecked((int)0xe0000243),
            SignatureOSAttributeMismatch = unchecked((int)0xe0000244),
            OnlyValidateViaAuthenticode = unchecked((int)0xe0000245)
        }

        private enum CR_STATUS : int
        {
            CR_SUCCESS = (0x00000000),
            CR_DEFAULT = (0x00000001),
            CR_OUT_OF_MEMORY = (0x00000002),
            CR_INVALID_POINTER = (0x00000003),
            CR_INVALID_FLAG = (0x00000004),
            CR_INVALID_DEVNODE = (0x00000005),
            CR_INVALID_DEVINST = CR_INVALID_DEVNODE,
            CR_INVALID_RES_DES = (0x00000006),
            CR_INVALID_LOG_CONF = (0x00000007),
            CR_INVALID_ARBITRATOR = (0x00000008),
            CR_INVALID_NODELIST = (0x00000009),
            CR_DEVNODE_HAS_REQS = (0x0000000A),
            CR_DEVINST_HAS_REQS = CR_DEVNODE_HAS_REQS,
            CR_INVALID_RESOURCEID = (0x0000000B),
            CR_DLVXD_NOT_FOUND = (0x0000000C),
            CR_NO_SUCH_DEVNODE = (0x0000000D),
            CR_NO_SUCH_DEVINST = CR_NO_SUCH_DEVNODE,
            CR_NO_MORE_LOG_CONF = (0x0000000E),
            CR_NO_MORE_RES_DES = (0x0000000F),
            CR_ALREADY_SUCH_DEVNODE = (0x00000010),
            CR_ALREADY_SUCH_DEVINST = CR_ALREADY_SUCH_DEVNODE,
            CR_INVALID_RANGE_LIST = (0x00000011),
            CR_INVALID_RANGE = (0x00000012),
            CR_FAILURE = (0x00000013),
            CR_NO_SUCH_LOGICAL_DEV = (0x00000014),
            CR_CREATE_BLOCKED = (0x00000015),
            CR_NOT_SYSTEM_VM = (0x00000016),
            CR_REMOVE_VETOED = (0x00000017),
            CR_APM_VETOED = (0x00000018),
            CR_INVALID_LOAD_TYPE = (0x00000019),
            CR_BUFFER_SMALL = (0x0000001A),
            CR_NO_ARBITRATOR = (0x0000001B),
            CR_NO_REGISTRY_HANDLE = (0x0000001C),
            CR_REGISTRY_ERROR = (0x0000001D),
            CR_INVALID_DEVICE_ID = (0x0000001E),
            CR_INVALID_DATA = (0x0000001F),
            CR_INVALID_API = (0x00000020),
            CR_DEVLOADER_NOT_READY = (0x00000021),
            CR_NEED_RESTART = (0x00000022),
            CR_NO_MORE_HW_PROFILES = (0x00000023),
            CR_DEVICE_NOT_THERE = (0x00000024),
            CR_NO_SUCH_VALUE = (0x00000025),
            CR_WRONG_TYPE = (0x00000026),
            CR_INVALID_PRIORITY = (0x00000027),
            CR_NOT_DISABLEABLE = (0x00000028),
            CR_FREE_RESOURCES = (0x00000029),
            CR_QUERY_VETOED = (0x0000002A),
            CR_CANT_SHARE_IRQ = (0x0000002B),
            CR_NO_DEPENDENT = (0x0000002C),
            CR_SAME_RESOURCES = (0x0000002D),
            CR_NO_SUCH_REGISTRY_KEY = (0x0000002E),
            CR_INVALID_MACHINENAME = (0x0000002F),
            CR_REMOTE_COMM_FAILURE = (0x00000030),
            CR_MACHINE_UNAVAILABLE = (0x00000031),
            CR_NO_CM_SERVICES = (0x00000032),
            CR_ACCESS_DENIED = (0x00000033),
            CR_CALL_NOT_IMPLEMENTED = (0x00000034),
            CR_INVALID_PROPERTY = (0x00000035),
            CR_DEVICE_INTERFACE_ACTIVE = (0x00000036),
            CR_NO_SUCH_DEVICE_INTERFACE = (0x00000037),
            CR_INVALID_REFERENCE_STRING = (0x00000038),
            CR_INVALID_CONFLICT_LIST = (0x00000039),
            CR_INVALID_INDEX = (0x0000003A),
            CR_INVALID_STRUCTURE_SIZE = (0x0000003B)
        }

        /// <summary>
        ///     Device Detailed Problem Codes
        /// </summary>
        private enum DN_PROBLEM : uint
        {
            NO_PROBLEM = 0,
            CM_PROB_NOT_CONFIGURED = (0x00000001),   // no config for device
            CM_PROB_DEVLOADER_FAILED = (0x00000002),   // service load failed
            CM_PROB_OUT_OF_MEMORY = (0x00000003),   // out of memory
            CM_PROB_ENTRY_IS_WRONG_TYPE = (0x00000004),   //
            CM_PROB_LACKED_ARBITRATOR = (0x00000005),   //
            CM_PROB_BOOT_CONFIG_CONFLICT = (0x00000006),   // boot config conflict
            CM_PROB_FAILED_FILTER = (0x00000007),   //
            CM_PROB_DEVLOADER_NOT_FOUND = (0x00000008),   // Devloader not found
            CM_PROB_INVALID_DATA = (0x00000009),   // Invalid ID
            CM_PROB_FAILED_START = (0x0000000A),   //
            CM_PROB_LIAR = (0x0000000B),   //
            CM_PROB_NORMAL_CONFLICT = (0x0000000C),   // config conflict
            CM_PROB_NOT_VERIFIED = (0x0000000D),   //
            CM_PROB_NEED_RESTART = (0x0000000E),   // requires restart
            CM_PROB_REENUMERATION = (0x0000000F),   //
            CM_PROB_PARTIAL_LOG_CONF = (0x00000010),   //
            CM_PROB_UNKNOWN_RESOURCE = (0x00000011),   // unknown res type
            CM_PROB_REINSTALL = (0x00000012),   //
            CM_PROB_REGISTRY = (0x00000013),   //
            CM_PROB_VXDLDR = (0x00000014),   // WINDOWS 95 ONLY
            CM_PROB_WILL_BE_REMOVED = (0x00000015),   // devinst will remove
            CM_PROB_DISABLED = (0x00000016),   // devinst is disabled
            CM_PROB_DEVLOADER_NOT_READY = (0x00000017),   // Devloader not ready
            CM_PROB_DEVICE_NOT_THERE = (0x00000018),   // device doesn't exist
            CM_PROB_MOVED = (0x00000019),   //
            CM_PROB_TOO_EARLY = (0x0000001A),   //
            CM_PROB_NO_VALID_LOG_CONF = (0x0000001B),   // no valid log config
            CM_PROB_FAILED_INSTALL = (0x0000001C),   // install failed
            CM_PROB_HARDWARE_DISABLED = (0x0000001D),   // device disabled
            CM_PROB_CANT_SHARE_IRQ = (0x0000001E),   // can't share IRQ
            CM_PROB_FAILED_ADD = (0x0000001F),   // driver failed add
            CM_PROB_DISABLED_SERVICE = (0x00000020),   // service's Start = 4
            CM_PROB_TRANSLATION_FAILED = (0x00000021),   // resource translation failed
            CM_PROB_NO_SOFTCONFIG = (0x00000022),   // no soft config
            CM_PROB_BIOS_TABLE = (0x00000023),   // device missing in BIOS table
            CM_PROB_IRQ_TRANSLATION_FAILED = (0x00000024),   // IRQ translator failed
            CM_PROB_FAILED_DRIVER_ENTRY = (0x00000025),   // DriverEntry() failed.
            CM_PROB_DRIVER_FAILED_PRIOR_UNLOAD = (0x00000026),   // Driver should have unloaded.
            CM_PROB_DRIVER_FAILED_LOAD = (0x00000027),   // Driver load unsuccessful.
            CM_PROB_DRIVER_SERVICE_KEY_INVALID = (0x00000028),   // Error accessing driver's service key
            CM_PROB_LEGACY_SERVICE_NO_DEVICES = (0x00000029),   // Loaded legacy service created no devices
            CM_PROB_DUPLICATE_DEVICE = (0x0000002A),   // Two devices were discovered with the same name
            CM_PROB_FAILED_POST_START = (0x0000002B),   // The drivers set the device state to failed
            CM_PROB_HALTED = (0x0000002C),   // This device was failed post start via usermode
            CM_PROB_PHANTOM = (0x0000002D),   // The devinst currently exists only in the registry
            CM_PROB_SYSTEM_SHUTDOWN = (0x0000002E),   // The system is shutting down
            CM_PROB_HELD_FOR_EJECT = (0x0000002F),   // The device is offline awaiting removal
            CM_PROB_DRIVER_BLOCKED = (0x00000030),   // One or more drivers is blocked from loading
            CM_PROB_REGISTRY_TOO_LARGE = (0x00000031),   // System hive has grown too large
            CM_PROB_SETPROPERTIES_FAILED = (0x00000032),   // Failed to apply one or more registry properties  
            CM_PROB_WAITING_ON_DEPENDENCY = (0x00000033),   // Device is stalled waiting on a dependency to start
            CM_PROB_UNSIGNED_DRIVER = (0x00000034),   // Failed load driver due to unsigned image.
            CM_PROB_USED_BY_DEBUGGER = (0x00000035),   // Device is being used by kernel debugger
            CM_PROB_DEVICE_RESET = (0x00000036),   // Device is being reset
            CM_PROB_CONSOLE_LOCKED = (0x00000037),   // Device is blocked while console is locked
            CM_PROB_NEED_CLASS_CONFIG = (0x00000038)   // Device needs extended class configuration to start
        }

        private enum _DEVICE_INSTALL_STATE
        {
            InstallStateInstalled,
            InstallStateNeedsReinstall,
            InstallStateFailedInstall,
            InstallStateFinishInstall
        }

        #endregion Enums

        #region Structs

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }        

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid interfaceClassGuid;
            public Int32 flags;
            private UIntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEVPROPKEY
        {
            public Guid fmtid;
            public uint pid;

            // from devpkey.h
            public static readonly DEVPROPKEY DEVPKEY_Device_Parent = new DEVPROPKEY { fmtid = new Guid("{4340A6C5-93FA-4706-972C-7B648008A5A7}"), pid = 8 };
            public static readonly DEVPROPKEY DEVPKEY_Device_Children = new DEVPROPKEY { fmtid = new Guid("{4340A6C5-93FA-4706-972C-7B648008A5A7}"), pid = 9 };
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STORAGE_DEVICE_NUMBER
        {
            public int DeviceType;
            public int DeviceNumber;
            public int PartitionNumber;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct SP_CLASSINSTALL_HEADER
        {
            public UInt32 cbSize;
            public UInt32 InstallFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader;
            public UInt32 StateChange;
            public UInt32 Scope;
            public UInt32 HwProfile;
        }
       
        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyChangeParameters
        {
            public int Size;
            // part of header. It's flattened out into 1 structure.
            public DiFunction DiFunction;
            public StateChangeAction StateChange;
            public Scopes Scope;
            public int HwProfile;
        }

        #endregion Structs

        #endregion Structs and Enums

        #endregion WinAPI

        #region consts
        private const int ERROR_INVALID_DATA = 13;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int MAX_PATH = 260;
        private const int ERROR_NO_MORE_ITEMS = 259;
        private const int MAX_DEVICE_ID_LEN = 200;
        private const uint DIF_PROPERTYCHANGE = 0x12;
        #endregion consts

        #region objects props
        private SP_DEVINFO_DATA _data;
        public string DeviceID { get; private set; }        
        public IntPtr _hDevInfo { get; private set; }
        #endregion objects props

        #region Inits
        private DeviceInfo(string DeviceID, IntPtr hDevInfo, SP_DEVINFO_DATA data)
        {
            this.DeviceID = DeviceID;
            this._hDevInfo = hDevInfo;
            this._data = data;
        }

        public void Dispose()
        {
            if (_hDevInfo != IntPtr.Zero)
            {
                SetupDiDestroyDeviceInfoList(_hDevInfo);
                _hDevInfo = IntPtr.Zero;
            };
        }
        #endregion Inits

        public static DeviceInfo Get(string DeviceID)
        {
            if (DeviceID == null) throw new ArgumentNullException("DeviceID");

            IntPtr hDevInfo = SetupDiGetClassDevs(IntPtr.Zero, DeviceID, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_DEVICEINTERFACE);
            if (hDevInfo == (IntPtr)INVALID_HANDLE_VALUE) return null;

            SP_DEVINFO_DATA data = new SP_DEVINFO_DATA();
            data.cbSize = Marshal.SizeOf(data);

            if (!SetupDiEnumDeviceInfo(hDevInfo, 0, ref data))
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
                int err = Marshal.GetLastWin32Error();
                if (err == ERROR_NO_MORE_ITEMS) { SetupDiDestroyDeviceInfoList(hDevInfo); return null; };
                throw new System.ComponentModel.Win32Exception(err);
            };

            return new DeviceInfo(DeviceID, hDevInfo, data);
        }                       

        public bool SetDeviceEnabled(bool enabled = true)
        {
            try
            {
                SP_DEVINFO_DATA devdata = new SP_DEVINFO_DATA();
                devdata.cbSize = (int)Marshal.SizeOf(devdata);

                for (uint i = 0; SetupDiEnumDeviceInfo(_hDevInfo, i, ref devdata); i++)
                    if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS) return false;

                SP_CLASSINSTALL_HEADER header = new SP_CLASSINSTALL_HEADER();
                header.cbSize = (UInt32)Marshal.SizeOf(header);
                header.InstallFunction = DIF_PROPERTYCHANGE;

                SP_PROPCHANGE_PARAMS propchangeparams = new SP_PROPCHANGE_PARAMS();
                propchangeparams.ClassInstallHeader = header;
                propchangeparams.StateChange = enabled ? (uint)1 : (uint)2;
                propchangeparams.Scope = 1; // Global
                propchangeparams.HwProfile = 0; // Default

                bool res = true;
                res &= SetupDiSetClassInstallParams(_hDevInfo, ref devdata, ref propchangeparams, (UInt32)Marshal.SizeOf(propchangeparams));
                res &= SetupDiChangeState(_hDevInfo, ref devdata);
                if (res) return res;
                else
                {
                    //try { res = SetDeviceEnabled(_hDevInfo, devdata, enabled); } catch { };
                    try { res = SetDeviceEnabled(_hDevInfo, DeviceID, enabled); } catch { };
                    return res;
                };
            }
            catch { };
            return false;
        }

        #region Properties
        public string ParentDeviceID
        {
            get
            {
                if (IsVistaOrHiger) return GetStringProperty(DEVPROPKEY.DEVPKEY_Device_Parent);

                uint parent;
                int cr = CM_Get_Parent(out parent, _data.DevInst, 0);
                if (cr != 0) throw new Exception("CM Error:" + cr);

                return GetDeviceID(parent);
            }
        }
        public string[] ChildrenDeviceIDs
        {
            get
            {
                if (IsVistaOrHiger) return GetStringListProperty(DEVPROPKEY.DEVPKEY_Device_Children);

                uint child;
                int cr = CM_Get_Child(out child, _data.DevInst, 0);
                if (cr != 0)
                    return new string[0];

                List<string> ids = new List<string>();
                ids.Add(GetDeviceID(child));
                do
                {
                    cr = CM_Get_Sibling(out child, child, 0);
                    if (cr != 0)
                        return ids.ToArray();

                    ids.Add(GetDeviceID(child));
                }
                while (true);
                return ids.ToArray();
            }
        }
        public bool IsUSB
        {
            get
            {
                if (this.DeviceID.ToUpper().StartsWith("USB")) return true;
                string parent = this.ParentDeviceID;
                if ((!string.IsNullOrEmpty(parent)) && parent.ToUpper().StartsWith("USB")) return true;
                string[] childs = ChildrenDeviceIDs;
                if ((childs != null) && (childs.Length > 0))
                    foreach (string c in childs)
                        if (c.ToUpper().StartsWith("USB"))
                            return true;
                return false;
            }
        }
        public bool IsDrive
        {
            get
            {
                try
                {
                    if (_hDevInfo == ((IntPtr)INVALID_HANDLE_VALUE)) return false;
                    if (DeviceID.ToUpper().StartsWith("USBTOR")) return true;

                    const string GUID_DEVINTERFACE_DISK = "{53F56307-B6BF-11D0-94F2-00A0C91EFB8B}";
                    Guid guid = new Guid(GUID_DEVINTERFACE_DISK);

                    SP_DEVICE_INTERFACE_DATA spdidd = new SP_DEVICE_INTERFACE_DATA();
                    spdidd.cbSize = Marshal.SizeOf(spdidd);

                    for (uint i = 0; SetupDiEnumDeviceInterfaces(_hDevInfo, IntPtr.Zero, ref guid, i, ref spdidd); i++)
                    {
                        uint dwSize = 0;
                        SetupDiGetDeviceInterfaceDetail(_hDevInfo, ref spdidd, IntPtr.Zero, 0, ref dwSize, IntPtr.Zero);
                        if (dwSize == 0 || dwSize > 1024) continue; // No Any Details
                        return true;
                    };
                }
                catch { };
                return false;
            }
        }

        public string DriveLetter
        {
            get
            {
                try
                {
                    if (_hDevInfo == ((IntPtr)INVALID_HANDLE_VALUE)) return "";

                    const string GUID_DEVINTERFACE_DISK = "{53F56307-B6BF-11D0-94F2-00A0C91EFB8B}";
                    Guid guid = new Guid(GUID_DEVINTERFACE_DISK);

                    SP_DEVICE_INTERFACE_DATA spdidd = new SP_DEVICE_INTERFACE_DATA();
                    spdidd.cbSize = Marshal.SizeOf(spdidd);

                    // Get all Interfaces on Device
                    for (uint i = 0; SetupDiEnumDeviceInterfaces(_hDevInfo, IntPtr.Zero, ref guid, i, ref spdidd); i++)
                    {
                        string DevicePath = "";
                        int DeviceNumber = -1;

                        // Get Device Interface Details Exists
                        {
                            uint dwSize = 0;
                            SetupDiGetDeviceInterfaceDetail(_hDevInfo, ref spdidd, IntPtr.Zero, 0, ref dwSize, IntPtr.Zero);
                            if (dwSize == 0 || dwSize > 1024)
                                continue; // No Any Details
                        };

                        // Get Device Path
                        {
                            SP_DEVINFO_DATA da = new SP_DEVINFO_DATA();
                            da.cbSize = Marshal.SizeOf(da);

                            SP_DEVICE_INTERFACE_DETAIL_DATA didd = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                            didd.cbSize = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize;

                            uint dwSize = 0;
                            if (!SetupDiGetDeviceInterfaceDetail(_hDevInfo, ref spdidd, ref didd, MAX_PATH, ref dwSize, ref da))
                                continue; // No Specified Details

                            DevicePath = didd.DevicePath;
                            if (da.DevInst == uint.MaxValue) continue;
                            if (string.IsNullOrEmpty(DevicePath)) continue;
                        };

                        // Open Device
                        {
                            IntPtr hDrive = CreateFile(DevicePath, (FileAccess)0, FileShare.Read, IntPtr.Zero, FileMode.Open, (FileAttributes)0, IntPtr.Zero);
                            if (hDrive == (IntPtr)INVALID_HANDLE_VALUE) continue;

                            uint dwSize = 0;
                            STORAGE_DEVICE_NUMBER sdn = new STORAGE_DEVICE_NUMBER();
                            if (DeviceIoControl(hDrive, (uint)EIOControlCode.StorageGetDeviceNumber, IntPtr.Zero, 0, ref sdn, (uint)Marshal.SizeOf(sdn), out dwSize, IntPtr.Zero))
                                DeviceNumber = sdn.DeviceNumber;

                            CloseHandle(hDrive);

                            if (DeviceNumber < 0) continue;
                        };

                        // Open Drives
                        foreach (DriveInfo drive in DriveInfo.GetDrives())
                        {
                            // if (drive.Name.StartsWith("C")) continue; // skip system drives

                            string vol = MSol.MachineManager.DeviceInfo.GetVolumeName(drive.Name);
                            string letterPath = @"\\.\" + drive.Name.Substring(0, 1) + ":";

                            IntPtr hVolume = CreateFile(letterPath, (FileAccess)0, FileShare.Read, IntPtr.Zero, FileMode.Open, (FileAttributes)0, IntPtr.Zero);
                            if (hVolume == (IntPtr)INVALID_HANDLE_VALUE) continue;

                            uint dwSize = 0;
                            STORAGE_DEVICE_NUMBER sdn = new STORAGE_DEVICE_NUMBER();
                            if (DeviceIoControl(hVolume, (uint)EIOControlCode.StorageGetDeviceNumber, IntPtr.Zero, 0, ref sdn, (uint)Marshal.SizeOf(sdn), out dwSize, IntPtr.Zero))
                            {
                                if (sdn.DeviceNumber == DeviceNumber)
                                {
                                    CloseHandle(hVolume);
                                    return drive.Name;
                                };
                            };

                            CloseHandle(hVolume);
                        };
                    };
                }
                catch { };
                return "";
            }
        }
        public Guid ClassGuid { get { return _data.ClassGuid; } }
        public string DeviceDesc { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_DEVICEDESC);  } }
        public string Class { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_CLASS); } }
        public string Driver { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_DRIVER); } }
        public string FriendlyName { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_FRIENDLYNAME); } }
        public string Location { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_LOCATION_INFORMATION); } }
        public string PhysicalDeviceObjectName { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_PHYSICAL_DEVICE_OBJECT_NAME); } }
        public string EnumeratorName { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_ENUMERATOR_NAME); } }
        public string DeviceType { get { return GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_DEVTYPE); } }        
        public string[] LocationPaths { get { return GetStringsProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_LOCATION_PATHS); } }
        public string[] HardwareIDs { get { return GetStringsProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_HARDWAREID); } }
        public string[] CompatibleIDs { get { return GetStringsProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_COMPATIBLEIDS); } }
        public int InstallState  {  get  {  return GetIntProperty(SetupDiGetDeviceRegistryPropertyEnum.SPDRP_INSTALL_STATE);  } }
        public string InstallStateS
        {
            get
            {
                _DEVICE_INSTALL_STATE iss = (_DEVICE_INSTALL_STATE)InstallState;
                return ((int)iss).ToString() + " " + iss.ToString();
            }
        }

        public string StatusS
        {
            get
            {
                string res = "Device or Status not found";
                //IntPtr devInst = IntPtr.Zero;
                //if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    DN_STATUS status;
                    DN_PROBLEM problem;
                    //CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, devInst, 0);

                    CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, (IntPtr)_data.DevInst, 0);

                    if (cmStat != CR_STATUS.CR_SUCCESS)
                        if ((cmStat == CR_STATUS.CR_NO_SUCH_DEVINST) || (cmStat == CR_STATUS.CR_NO_SUCH_VALUE))
                            return res;
                    return ((uint)status).ToString() + " " + status.ToString();
                };
                //return res;
            }
        }

        public uint Status
        {
            get
            {
                uint res = 0;
                //IntPtr devInst = IntPtr.Zero;
                //if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    DN_STATUS status;
                    DN_PROBLEM problem;
                    //CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, devInst, 0);

                    CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, (IntPtr)_data.DevInst, 0);

                    if (cmStat != CR_STATUS.CR_SUCCESS)
                        if ((cmStat == CR_STATUS.CR_NO_SUCH_DEVINST) || (cmStat == CR_STATUS.CR_NO_SUCH_VALUE))
                            return res;
                    return (uint)status;
                };
                //return res;
            }
        }

        public string ProblemS
        {
            get
            {                
                string res = "Device or Problem not found";
                //IntPtr devInst = IntPtr.Zero;
                //if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    DN_STATUS status;
                    DN_PROBLEM problem;
                    //CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, devInst, 0);

                    CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, (IntPtr)_data.DevInst, 0);

                    if (cmStat != CR_STATUS.CR_SUCCESS)
                        if ((cmStat == CR_STATUS.CR_NO_SUCH_DEVINST) || (cmStat == CR_STATUS.CR_NO_SUCH_VALUE))
                            return res;
                    return ((uint)problem).ToString() + " " + problem.ToString();
                };
                //return res;
            }
        }
        
        public uint Problem
        {
            get
            {
                uint res = 0;
                //IntPtr devInst = IntPtr.Zero;
                //if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    DN_STATUS status;
                    DN_PROBLEM problem;
                    //CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, devInst, 0);

                    CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, (IntPtr)_data.DevInst, 0);

                    if (cmStat != CR_STATUS.CR_SUCCESS)
                        if ((cmStat == CR_STATUS.CR_NO_SUCH_DEVINST) || (cmStat == CR_STATUS.CR_NO_SUCH_VALUE))
                            return res;
                    return (uint)problem;
                };
                //return res;
            }
        }

        public bool NoDriver
        {
            get
            {
                uint problem = Problem;
                return problem == 0x1C || problem == 0x1F;
            }
        }

        public bool DriverFail
        {
            get
            {
                uint problem = Problem;
                return (problem == 0x01) || (problem == 0x02) || (problem == 0x04) || (problem == 0x08) || (problem == 0x09) ||
                    (problem == 0x0C) || (problem == 0x0D) || (problem == 0x10) || (problem == 0x11) || (problem == 0x12) || (problem == 0x13) ||
                    (problem == 0x1B) || (problem == 0x1C) || (problem == 0x1F) || (problem == 0x21) || (problem == 0x22) || (problem == 0x25) ||
                    (problem == 0x27) || (problem == 0x28) || (problem == 0x29) || (problem == 0x2A) || (problem == 0x2B) || (problem == 0x2D) ||
                    (problem == 0x30) || (problem == 0x32) || (problem == 0x34) || (problem == 0x38);
            }
        }

        public bool DriverBad
        {
            get
            {
                uint problem = Problem;
                return (problem == 0x01) || (problem == 0x02) || (problem == 0x04) || (problem == 0x08) || (problem == 0x09) ||
                            (problem == 0x0C) || (problem == 0x0D) || (problem == 0x10) || (problem == 0x11) || (problem == 0x12) || (problem == 0x13) ||
                            (problem == 0x1B) || (problem == 0x21) || (problem == 0x22) || (problem == 0x25) ||
                            (problem == 0x27) || (problem == 0x28) || (problem == 0x29) || (problem == 0x2A) || (problem == 0x2B) || (problem == 0x2D) ||
                            (problem == 0x30) || (problem == 0x32) || (problem == 0x34) || (problem == 0x38);
            }
        }

        public bool Disabled
        {
            get
            {
                uint problem = Problem;
                return (problem == 0x16) || (problem == 0x1D);
            }
        }

        public static void GetDeviceDetailedStatus(string DeviceID, ref string Status, ref uint Problem)
        {
            IntPtr devInst = IntPtr.Zero;
            try
            {                
                if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    DN_STATUS status;
                    DN_PROBLEM problem;
                    CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, devInst, 0);
                    Problem = (uint)problem;

                    if (cmStat != CR_STATUS.CR_SUCCESS)
                    {
                        if ((cmStat == CR_STATUS.CR_NO_SUCH_DEVINST) || (cmStat == CR_STATUS.CR_NO_SUCH_VALUE))
                        {
                            Status = "Device or Status not found";
                            return;
                        };
                    };

                    bool HasProblem = (status & DN_STATUS.DN_HAS_PROBLEM) != 0;
                    bool Disabled = HasProblem && (problem == DN_PROBLEM.CM_PROB_DISABLED);
                    if (Disabled)
                    {
                        Status = "Device is Disable";
                        return;
                    };

                    if (HasProblem && (Status.ToUpper() != "OK"))
                    {
                        Status = "Error";
                        return;
                    };
                    if (string.IsNullOrEmpty(Status)) Status = "OK";
                }
                else
                {
                    if (string.IsNullOrEmpty(Status)) Status = "Unknown";
                };
            }
            catch { if (string.IsNullOrEmpty(Status)) Status = "Unknown"; }
            finally { if (devInst != IntPtr.Zero) SetupDiDestroyDeviceInfoList(devInst); };
        }

        public static bool GetDeviceError(string DeviceID, out uint Problem, out string sProblem, bool errNoDevice = false, bool errDeviceDisabled = false)
        {
            Problem = 0;
            sProblem = "";
            IntPtr devInst = IntPtr.Zero;
            try
            {
                if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    DN_STATUS status;
                    DN_PROBLEM problem;
                    CR_STATUS cmStat = CM_Get_DevNode_Status(out status, out problem, devInst, 0);
                    Problem = (uint)problem;
                    sProblem = problem.ToString();

                    if (cmStat != CR_STATUS.CR_SUCCESS)
                        if ((cmStat == CR_STATUS.CR_NO_SUCH_DEVINST) || (cmStat == CR_STATUS.CR_NO_SUCH_VALUE))
                            return errNoDevice;

                    bool HasProblem = (status & DN_STATUS.DN_HAS_PROBLEM) != 0;
                    bool Disabled = HasProblem && (problem == DN_PROBLEM.CM_PROB_DISABLED);
                    if (Disabled) return errDeviceDisabled;
                    if (HasProblem) return true;
                    return false;
                }
                else return errNoDevice;
            }
            catch { return errNoDevice; }
            finally { if (devInst != IntPtr.Zero) SetupDiDestroyDeviceInfoList(devInst); };
        }

        public static string GetDeviceName(string DeviceID)
        {
            IntPtr devInst = IntPtr.Zero;
            try
            {
                if ((CM_Locate_DevNodeW(ref devInst, DeviceID, 0) == 0) && (devInst != IntPtr.Zero))
                {
                    SP_DEVINFO_DATA devdata = new SP_DEVINFO_DATA();
                    devdata.cbSize = (int)Marshal.SizeOf(devdata);

                    if (!SetupDiEnumDeviceInfo(devInst, 0, ref devdata)) return null;
                    return GetStringPropertyForDevice(devInst, devdata, (uint)SetupDiGetDeviceRegistryPropertyEnum.SPDRP_FRIENDLYNAME).Trim();
                };
            }
            catch { }
            finally { if (devInst != IntPtr.Zero) SetupDiDestroyDeviceInfoList(devInst); };
            return null;
        }

        public static object GetDeviceRegistryValue(string DeviceID, string Key, object defValue = null)
        {
            try
            {
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\" + DeviceID);
                object res = rk.GetValue(Key, defValue);
                rk.Close();
                return res;
            }
            catch { return defValue; };            
        }

        public static bool SaveDeviceIcon(string DeviceID, string ClassGuid, string fileName)
        {
            if (string.IsNullOrEmpty(DeviceID)) return false;
            if (string.IsNullOrEmpty(fileName)) return false;

            string classGuid = string.IsNullOrEmpty(ClassGuid) ? GetDeviceRegistryValue(DeviceID, "ClassGUID")?.ToString() : ClassGuid;
            if(string.IsNullOrEmpty(classGuid))
            {
                int iof = DeviceID.IndexOf(@"\");
                if (iof > 0)
                {
                    string group = DeviceID.Substring(0, iof);
                    if (DeviceClasses.ContainsKey(group)) classGuid = "{" + DeviceClasses[group].ToString() + "}";
                };
            };
            if (string.IsNullOrEmpty(classGuid)) return false; // No ClassGuid

            string iconPath = null;
            try
            {
                string keyPath = @"SYSTEM\CurrentControlSet\Control\Class\" + classGuid;
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath);
                iconPath = ((string[])rk.GetValue("IconPath", null))?[0];
                rk.Close();
            }
            catch { };
            if (string.IsNullOrEmpty(iconPath)) return false; // No IconPath

            // Get IconIndex
            int iconIndex = 0;
            {
                int dp = iconPath.IndexOf(",");
                if (dp > 0)
                {
                    string ic = iconPath.Substring(dp + 1);
                    iconPath = iconPath.Substring(0, dp);
                    int.TryParse(ic, out iconIndex);
                };
            };


            // using DLL Call
            IntPtr hIcon = IntPtr.Zero;
            try
            {                                                                
                ExtractIconEx(iconPath, iconIndex, out hIcon, IntPtr.Zero, 1);
                if (hIcon != IntPtr.Zero)
                {
                    System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(hIcon);
                    System.Drawing.Bitmap bmp = icon.ToBitmap();
                    bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                    bmp.Dispose();
                    icon.Dispose();
                    return true;
                };
            }
            catch { }
            finally { if (hIcon != IntPtr.Zero) DestroyIcon(hIcon); };
            return false;
        }        
        
        #endregion Properties        

        #region Private Properties
        private string[] GetStringListProperty(DEVPROPKEY key)
        {
            int type;
            int size;
            SetupDiGetDeviceProperty(_hDevInfo, ref _data, ref key, out type, IntPtr.Zero, 0, out size, 0);
            if (size == 0) return new string[0];

            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                if (!SetupDiGetDeviceProperty(_hDevInfo, ref _data, ref key, out type, buffer, size, out size, 0))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                List<string> strings = new List<string>();
                IntPtr current = buffer;
                do
                {
                    string s = Marshal.PtrToStringUni(current);
                    if (string.IsNullOrEmpty(s))
                        break;

                    strings.Add(s);
                    current += (1 + s.Length) * 2;
                }
                while (true);
                return strings.ToArray();
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            };
        }

        private string GetStringProperty(DEVPROPKEY key)
        {
            int type;
            int size;

            SetupDiGetDeviceProperty(_hDevInfo, ref _data, ref key, out type, IntPtr.Zero, 0, out size, 0);
            if (size == 0) return null;

            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                if (!SetupDiGetDeviceProperty(_hDevInfo, ref _data, ref key, out type, buffer, size, out size, 0))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                return Marshal.PtrToStringUni(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            };
        }        

        private string GetStringProperty(SetupDiGetDeviceRegistryPropertyEnum propID)
        {
            try
            {
                SP_DEVINFO_DATA devdata = new SP_DEVINFO_DATA();
                devdata.cbSize = (int)Marshal.SizeOf(devdata);

                for (uint i = 0; SetupDiEnumDeviceInfo(_hDevInfo, i, ref devdata); i++)
                    if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS) return null;

                return GetStringPropertyForDevice(_hDevInfo, devdata, (uint)propID);
            }
            catch
            { };
            return null;
        }

        private string[] GetStringsProperty(SetupDiGetDeviceRegistryPropertyEnum propID)
        {
            try
            {
                SP_DEVINFO_DATA devdata = new SP_DEVINFO_DATA();
                devdata.cbSize = (int)Marshal.SizeOf(devdata);

                for (uint i = 0; SetupDiEnumDeviceInfo(_hDevInfo, i, ref devdata); i++)
                    if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS) return null;

                return GetStringsPropertyForDevice(_hDevInfo, devdata, (uint)propID);
            }
            catch
            { };
            return null;
        }

        private int GetIntProperty(SetupDiGetDeviceRegistryPropertyEnum propID)
        {
            try
            {
                SP_DEVINFO_DATA devdata = new SP_DEVINFO_DATA();
                devdata.cbSize = (int)Marshal.SizeOf(devdata);

                for (uint i = 0; SetupDiEnumDeviceInfo(_hDevInfo, i, ref devdata); i++)
                    if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS) return 0;

                return GetIntPropertyForDevice(_hDevInfo, devdata, (uint)propID);
            }
            catch
            { };
            return 0;
        }
        #endregion Private Properties

        #region Static
        public static string GetVolumeName(string MountPoint)
        {
            // MountPoint = "C:\";
            const int MaxVolumeNameLength = 100;
            StringBuilder sb = new StringBuilder(MaxVolumeNameLength);
            if (!GetVolumeNameForVolumeMountPoint(MountPoint, sb, (uint)MaxVolumeNameLength))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            string vol = sb.ToString();

            //// TEST
            //{
            //    char[] buffer = new char[256];
            //    uint dWord = 0;
            //    if (!GetVolumePathNamesForVolumeName(vol, buffer, (uint)buffer.Length, out dWord))
            //    {
            //        buffer = new char[dWord];
            //        GetVolumePathNamesForVolumeName(vol, buffer, (uint)buffer.Length, out dWord);
            //    };
            //    if (buffer[0] != '\0')
            //    {
            //        string letter = new string(buffer).Trim('\0');
            //    };
            //};

            return vol;
        }

        public static bool SetDeviceEnabled(DeviceInfo devInfo, bool enable = true)
        {
            return SetDeviceEnabled(devInfo._hDevInfo, devInfo._data, enable);
        }

        public static bool SetDeviceEnabled(IntPtr hDevInfo, string DeviceID, bool enabled = true)
        {
            Func<SP_DEVINFO_DATA[]> GetDeviceInfoData = () =>
            {
                List<SP_DEVINFO_DATA> data = new List<SP_DEVINFO_DATA>();
                SP_DEVINFO_DATA did = new SP_DEVINFO_DATA();
                int didSize = Marshal.SizeOf(did);
                did.cbSize = didSize;
                int index = 0;
                while (SetupDiEnumDeviceInfo(hDevInfo, (uint)index, ref did))
                {
                    data.Add(did);
                    index += 1;
                    did = new SP_DEVINFO_DATA();
                    did.cbSize = didSize;
                };
                return data.ToArray();
            };

            Func<SP_DEVINFO_DATA[],int> GetIndexOfInstance = (SP_DEVINFO_DATA[] diData) =>
            {
                const int ERROR_INSUFFICIENT_BUFFER = 122;
                for (int i = 0; i < diData.Length; i++)
                {
                    StringBuilder sb = new StringBuilder(1);
                    int requiredSize = 0;
                    bool result = SetupDiGetDeviceInstanceId(hDevInfo, ref diData[i], sb, sb.Capacity, out requiredSize);
                    if (result == false && Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                    {
                        sb.Capacity = requiredSize;
                        result = SetupDiGetDeviceInstanceId(hDevInfo, ref diData[i], sb, sb.Capacity, out requiredSize);
                    };
                    if (result == false) throw new System.ComponentModel.Win32Exception();
                    if (DeviceID.Equals(sb.ToString())) return i;
                };
                return -1;
            };

            try
            {
                // hDevInfo = SetupDiGetClassDevs(ref ClassGuid, null, IntPtr.Zero, SetupDiGetClassDevsFlags.Present);
                SP_DEVINFO_DATA[] diData = GetDeviceInfoData();
                // Find the index of our instance. i.e. the touchpad mouse - I have 3 mice attached...
                int index = 0;
                if((diData.Length > 1) && (!string.IsNullOrEmpty(DeviceID))) index = GetIndexOfInstance(diData);
                return SetDeviceEnabled(hDevInfo, diData[index], enabled);
            }
            catch {};
            return false;            
        }        

        private static bool SetDeviceEnabled(IntPtr hDevInfo, SP_DEVINFO_DATA diData, bool enable = true)
        {
            bool bWow64 = false;
            if (IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out bWow64) && bWow64)
            {
                IntPtr OldValue = IntPtr.Zero;
                Wow64DisableWow64FsRedirection(out OldValue);
            };

            PropertyChangeParameters cparams = new PropertyChangeParameters();
            cparams.Size = 8;
            cparams.DiFunction = DiFunction.PropertyChange;
            cparams.Scope = Scopes.Global;
            cparams.StateChange = enable ? StateChangeAction.Enable : StateChangeAction.Disable;
            
            bool res = SetupDiSetClassInstallParams(hDevInfo, ref diData, ref cparams, Marshal.SizeOf(cparams));
            if (res == false) throw new System.ComponentModel.Win32Exception();
            res = SetupDiChangeState(hDevInfo, ref diData);
            if (res) return true;
            res = SetupDiCallClassInstaller(DiFunction.PropertyChange, hDevInfo, ref diData);
            if (res == false)
            {
                int err = Marshal.GetLastWin32Error();
                if (err == (int)SetupApiError.NotDisableable)
                    throw new ArgumentException("Device can't be disabled (programmatically or in Device Manager).");
                else if (err >= (int)SetupApiError.NoAssociatedClass && err <= (int)SetupApiError.OnlyValidateViaAuthenticode)
                    throw new System.ComponentModel.Win32Exception("SetupAPI error: " + ((SetupApiError)err).ToString());
                else
                    throw new System.ComponentModel.Win32Exception();
            };
            return true;
        }

        public static string GetStringPropertyForDevice(DeviceInfo devInfo, uint propId)
        {
            return GetStringPropertyForDevice(devInfo._hDevInfo, devInfo._data, propId);
        }

        private static string GetStringPropertyForDevice(IntPtr hDevInfo, SP_DEVINFO_DATA devdata, uint propId)
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                uint buflen = 2048;
                buffer = Marshal.AllocHGlobal((int)buflen);

                uint proptype = 0;
                uint outsize = 0;

                SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref devdata, propId, out proptype, buffer, buflen, ref outsize);
                byte[] lbuffer = new byte[outsize];
                Marshal.Copy(buffer, lbuffer, 0, (int)outsize);
                int errcode = Marshal.GetLastWin32Error();
                if (errcode == ERROR_INVALID_DATA) return null;
                return Encoding.Unicode.GetString(lbuffer).Trim('\0');
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            };
        }

        private static string[] GetStringsPropertyForDevice(IntPtr hDevInfo, SP_DEVINFO_DATA devdata, uint propId)
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                uint buflen = 2048;
                buffer = Marshal.AllocHGlobal((int)buflen);

                uint proptype = 0;
                uint outsize = 0;

                SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref devdata, propId, out proptype, buffer, buflen, ref outsize);
                byte[] lbuffer = new byte[outsize];
                Marshal.Copy(buffer, lbuffer, 0, (int)outsize);
                int errcode = Marshal.GetLastWin32Error();
                if (errcode == ERROR_INVALID_DATA) return null;
                string[] bres = Encoding.Unicode.GetString(lbuffer).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                return bres;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            };
        }

        private static int GetIntPropertyForDevice(IntPtr hDevInfo, SP_DEVINFO_DATA devdata, uint propId)
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                uint buflen = 8;
                buffer = Marshal.AllocHGlobal((int)buflen);

                uint proptype = 0;
                uint outsize = 0;

                SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref devdata, propId, out proptype, buffer, buflen, ref outsize);
                byte[] lbuffer = new byte[outsize];
                Marshal.Copy(buffer, lbuffer, 0, (int)outsize);
                int errcode = Marshal.GetLastWin32Error();
                if (errcode == ERROR_INVALID_DATA) return 0;
                int res = BitConverter.ToInt32(lbuffer, 0);
                return res;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            };
        }

        private static string GetDeviceID(uint inst)
        {
            IntPtr buffer = Marshal.AllocHGlobal(MAX_DEVICE_ID_LEN + 1);
            int cr = CM_Get_Device_ID(inst, buffer, MAX_DEVICE_ID_LEN + 1, 0);
            if (cr != 0) throw new Exception("CM Error:" + cr);

            try
            {
                return Marshal.PtrToStringAnsi(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            };
        }

        private static bool IsVistaOrHiger
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.CompareTo(new Version(6, 0)) >= 0);
            }
        }
        #endregion Static

        #region ListDevices

        public static readonly Dictionary<string, Guid> DeviceClasses = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
        {
            {"All", Guid.Empty },
            {"1394", new Guid("{6bdd1fc1-810f-11d0-bec7-08002be2092f}")},
            {"1394Debug", new Guid("{66f250d6-7801-4a64-b139-eea80a450b24}")},
            {"61883", new Guid("{7ebefbc0-3200-11d2-b4c2-00a0C9697d07}")},
            {"ActivityMonitor", new Guid("{b86dff51-a31e-4bac-b3cf-e8cfe75c9fc2}")},
            {"Adapter", new Guid("{4d36e964-e325-11ce-bfc1-08002be10318}")},
            {"AntiVirus", new Guid("{b1d1a169-c54f-4379-81db-bee7d88d7454}")},
            {"APMSupport", new Guid("{d45b1c18-c8fa-11d1-9f77-0000f805f530}")},
            {"AudioEndpoint", new Guid("{c166523c-fe0c-4a94-a586-f1a80cfbbf3e}")},
            {"AudioProcessingObject", new Guid("{5989fce8-9cd0-467d-8a6a-5419e31529d4}")},
            {"AVC", new Guid("{c06ff265-ae09-48f0-812c-16753d7cba83}")},
            {"BarcodeScanner", new Guid("{c243ffbd-3afc-45e9-b3d3-2ba18bc7ebc5}")},
            {"BasicDisplay", new Guid("{6fae73b7-b735-4b50-a0da-0dc2484b1f1a}")},
            {"Battery", new Guid("{72631e54-78a4-11d0-bcf7-00aa00b7b32a}")},
            {"Biometric", new Guid("{53D29EF7-377C-4D14-864B-EB3A85769359}")},
            {"Bluetooth", new Guid("{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}")},
            {"Camera", new Guid("{ca3e7ab9-b4c3-4ae6-8251-579ef933890f}")},
            {"CDROM", new Guid("{4d36e965-e325-11ce-bfc1-08002be10318}")},
            {"CFSMetadataServer", new Guid("{cdcf0939-b75b-4630-bf76-80f7ba655884}")},
            {"Compression", new Guid("{f3586baf-b5aa-49b5-8d6c-0569284c639f}")},
            {"ComputeAccelerator", new Guid("{f01a9d53-3ff6-48d2-9f97-c8a7004be10c}")},
            {"Computer", new Guid("{4d36e966-e325-11ce-bfc1-08002be10318}")},
            {"ContentScreener", new Guid("{3e3f0674-c83c-4558-bb26-9820e1eba5c5}")},
            {"ContinuousBackup", new Guid("{71aa14f8-6fad-4622-ad77-92bb9d7e6947}")},
            {"CopyProtection", new Guid("{89786ff1-9c12-402f-9c9e-17753c7f4375}")},
            {"Decoder", new Guid("{6bdd1fc2-810f-11d0-bec7-08002be2092f}")},
            {"DigitalMediaDevices", new Guid("{14b62f50-3f15-11dd-ae16-0800200c9a66}")},
            {"Disk", new Guid("{53F56307-B6BF-11D0-94F2-00A0C91EFB8B}") },
            {"DiskDrive", new Guid("{4d36e967-e325-11ce-bfc1-08002be10318}")},            
            {"Display", new Guid("{4d36e968-e325-11ce-bfc1-08002be10318}")},
            {"Dot4", new Guid("{48721b56-6795-11d2-b1a8-0080c72e74a2}")},
            {"Dot4Print", new Guid("{49ce6ac8-6f86-11d2-b1e5-0080c72e74a2}")},
            {"DXGKrnl", new Guid("{1264760f-a5c8-4bfe-b314-d56a7b44a362}")},
            {"EhStorSilo", new Guid("{9da2b80f-f89f-4a49-a5c2-511b085b9e8a}")},
            {"Encryption", new Guid("{a0a701c0-a511-42ff-aa6c-06dc0395576f}")},
            {"Enum1394", new Guid("{c459df55-db08-11d1-b009-00a0c9081ff6}")},
            {"Extension", new Guid("{e2f84ce7-8efa-411c-aa69-97454ca4cb57}")},
            {"FDC", new Guid("{4d36e969-e325-11ce-bfc1-08002be10318}")},
            {"Firmware", new Guid("{f2e7dd72-6468-4e36-b6f1-6488f42c1b52}")},
            {"FloppyDisk", new Guid("{4d36e980-e325-11ce-bfc1-08002be10318}")},
            {"FSFilterSystem", new Guid("{5d1b9aaa-01e2-46af-849f-272b3f324c46}")},
            {"fvevol_1", new Guid("{3163c566-d381-4467-87bc-a65a18d5b649}")},
            {"GPS", new Guid("{6bdd1fc3-810f-11d0-bec7-08002be2092f}")},
            {"HDC", new Guid("{4d36e96a-e325-11ce-bfc1-08002be10318}")},
            {"HidCashDrawer", new Guid("{772e18f2-8925-4229-a5ac-6453cb482fda}")},
            {"HIDClass", new Guid("{745a17a0-74d3-11d0-b6fe-00a0c90f57da}")},
            {"HidLineDisplay", new Guid("{4fc9541c-0fe6-4480-a4f6-9495a0d17cd2}")},
            {"HidMsr", new Guid("{2a9fe532-0cdc-44f9-9827-76192f2ca2fb}")},
            {"Holographic", new Guid("{d612553d-06b1-49ca-8938-e39ef80eb16f}")},
            {"HSM", new Guid("{d546500a-2aeb-45f6-9482-f4b1799c3177}")},
            {"Image", new Guid("{6bdd1fc6-810f-11d0-bec7-08002be2092f}")},
            {"Infrared", new Guid("{6bdd1fc5-810f-11d0-bec7-08002be2092f}")},
            {"Infrastructure", new Guid("{e55fa6f9-128c-4d04-abab-630c74b1453a}")},
            {"InsydeDevice", new Guid("{416c2604-443b-436f-9e1d-607bdc3cc785}")},
            {"Keyboard", new Guid("{4d36e96b-e325-11ce-bfc1-08002be10318}")},
            {"LegacyDriver", new Guid("{8ecc055d-047f-11d1-a537-0000f8753ed1}")},
            {"Media Center Extender", new Guid("{43675d81-502a-4a82-9f84-b75f418c5dea}")},
            {"Media", new Guid("{4d36e96c-e325-11ce-bfc1-08002be10318}")},
            {"MediumChanger", new Guid("{ce5939ae-ebde-11d0-b181-0000f8753ec4}")},
            {"Memory", new Guid("{5099944a-f6b9-4057-a056-8c550228544c}")},
            {"Miracast", new Guid("{d421b08e-6d16-41ca-9c4d-9147e5ac98e0}")},
            {"Modem", new Guid("{4d36e96d-e325-11ce-bfc1-08002be10318}")},
            {"Monitor", new Guid("{4d36e96e-e325-11ce-bfc1-08002be10318}")},
            {"Mouse", new Guid("{4d36e96f-e325-11ce-bfc1-08002be10318}")},
            {"MTD", new Guid("{4d36e970-e325-11ce-bfc1-08002be10318}")},
            {"Multifunction", new Guid("{4d36e971-e325-11ce-bfc1-08002be10318}")},
            {"MultiportSerial", new Guid("{50906cb8-ba12-11d1-bf5d-0000f805f530}")},
            {"Net", new Guid("{4d36e972-e325-11ce-bfc1-08002be10318}")},
            {"NetClient", new Guid("{4d36e973-e325-11ce-bfc1-08002be10318}")},
            {"NetDriver", new Guid("{87ef9ad1-8f70-49ee-b215-ab1fcadcbe3c}")},
            {"NetService", new Guid("{4d36e974-e325-11ce-bfc1-08002be10318}")},
            {"NetTrans", new Guid("{4d36e975-e325-11ce-bfc1-08002be10318}")},
            {"NoDriver", new Guid("{4d36e976-e325-11ce-bfc1-08002be10318}")},
            {"OpenFileBackup", new Guid("{f8ecafa6-66d1-41a5-899b-66585d7216b7}")},
            {"OposLegacyDevice", new Guid("{5aea001d-9372-4ed7-97f3-b79bf15a53c5}")},
            {"PCMCIA", new Guid("{4d36e977-e325-11ce-bfc1-08002be10318}")},
            {"PerceptionSimulation", new Guid("{645ad99b-1344-4316-837a-08a3e73db222}")},
            {"PhysicalQuotaManagement", new Guid("{6a0a8e78-bba6-4fc4-a709-1e33cd09d67e}")},
            {"PNPPrinters", new Guid("{4658ee7e-f050-11d1-b6bd-00c04fa372a7}")},
            {"Ports", new Guid("{4d36e978-e325-11ce-bfc1-08002be10318}")},
            {"POSPrinter", new Guid("{c7bc9b22-21f0-4f0d-9bb6-66c229b8cd33}")},
            {"Printer", new Guid("{4d36e979-e325-11ce-bfc1-08002be10318}")},
            {"PrinterUpgrade", new Guid("{4d36e97a-e325-11ce-bfc1-08002be10318}")},
            {"PrintQueue", new Guid("{1ed2bbf9-11f0-4084-b21f-ad83a8e6dcdc}")},
            {"Processor", new Guid("{50127dc3-0f36-415e-a6cc-4cb3be910b65}")},
            {"Proximity", new Guid("{5630831c-06c9-4856-b327-f5d32586e060}")},
            {"QuotaManagement", new Guid("{8503c911-a6c7-4919-8f79-5028f5866b0c}")},
            {"RDCamera", new Guid("{b2728d24-ac56-42db-9e02-8edaf5db652f}")},
            {"rdpbus", new Guid("{a3e32dba-ba89-4f17-8386-2d0127fbd4cc}")},
            {"RDPDR", new Guid("{091bc97e-2352-4362-a539-10a6d8ff7596}")},
            {"RDPDR_2", new Guid("{cc41eba2-ab57-4f4e-8c3d-1bc33b1e74e3}")},
            {"RdpVideoMiniport", new Guid("{81c87465-de07-4efc-9d93-61e891d52fd2}")},
            {"RemotePosDevice", new Guid("{13e42dfa-85d9-424d-8646-28a70f864f9c}")},
            {"Replication", new Guid("{48d3ebc4-4cf8-48ff-b869-9c68ad42eb9f}")},
            {"SBP2", new Guid("{d48179be-ec20-11d1-b6b8-00c04fa372a7}")},
            {"ScmDisk", new Guid("{53966cb1-4d46-4166-bf23-c522403cd495}")},
            {"ScmVolume", new Guid("{53ccb149-e543-4c84-b6e0-bce4f6b7e806}")},
            {"SCSIAdapter", new Guid("{4d36e97b-e325-11ce-bfc1-08002be10318}")},
            {"SDHost", new Guid("{a0a588a4-c46f-4b37-b7ea-c82fe89870c6}")},
            {"SecurityAccelerator", new Guid("{268c95a1-edfe-11d3-95c3-0010dc4050a5}")},
            {"Securitydevices", new Guid("{d94ee5d8-d189-4994-83d2-f68d7d41b0e6}")},
            {"SecurityEnhancer", new Guid("{d02bc3da-0c8e-4945-9bd5-f1883c226c8c}")},
            {"Sensor", new Guid("{5175d334-c371-4806-b3ba-71fd53c9258d}")},
            {"SmartCard", new Guid("{990a2bd7-e738-46c7-b26f-1cf8fb9f1391}")},
            {"SmartCardFilter", new Guid("{db4f6ddd-9c0e-45e4-9597-78dbbad0f412}")},
            {"SmartCardReader", new Guid("{50dd5230-ba8a-11d1-bf5d-0000f805f530}")},
            {"SmrDisk", new Guid("{53487c23-680f-4585-acc3-1f10d6777e82}")},
            {"SmrVolume", new Guid("{53b3cf03-8f5a-4788-91b6-d19ed9fcccbf}")},
            {"SoftwareComponent", new Guid("{5c4c3332-344d-483c-8739-259e934c9cc8}")},
            {"SoftwareDevice", new Guid("{62f9c741-b25a-46ce-b54c-9bccce08b6f2}")},
            {"Sound", new Guid("{4d36e97c-e325-11ce-bfc1-08002be10318}")},
            {"System", new Guid("{4d36e97d-e325-11ce-bfc1-08002be10318}")},
            {"SystemRecovery", new Guid("{2db15374-706e-4131-a0c7-d7c78eb0289a}")},
            {"TapeDrive", new Guid("{6d807884-7d21-11cf-801c-08002be10318}")},
            {"TS_Generic", new Guid("{88a1c342-4539-11d3-b88d-00c04fad5171}")},
            {"UCM", new Guid("{e6f1aa1c-7f3b-4473-b2e8-c97d8ac71d53}")},
            {"Undelete", new Guid("{fe8f1572-c67a-48c0-bbac-0b5c6d66cafb}")},
            {"Unknown", new Guid("{4d36e97e-e325-11ce-bfc1-08002be10318}")},
            {"USB", new Guid("{36fc9e60-c465-11cf-8056-444553540000}")},
            {"USBDevice", new Guid("{88BAE032-5A81-49f0-BC3D-A4FF138216D6}")},
            {"USBFunctionController", new Guid("{bbbe8734-08fa-4966-b6a6-4e5ad010cdd7}")},
            {"USBMass", new Guid("{a5dcbf10-6530-11d2-901f-00c04fb951ed}")},
            {"Virtualization", new Guid("{f75a86c0-10d8-4c3a-b233-ed60e4cdfaac}")},
            {"Volume", new Guid("{71a27cdd-812a-11d0-bec7-08002be2092f}")},
            {"VolumeSnapshot", new Guid("{533c5b84-ec70-11d2-9505-00c04F79deaf}")},
            {"WCEUSBS", new Guid("{25dbce51-6c8f-4a72-8a6d-b54c2b4fc835}")},
            {"WPD", new Guid("{eec5ad98-8080-425f-922a-dabf3de3f69a}")},
            {"WSDPrintDevice", new Guid("{c30ecea0-11ef-4ef9-b02e-6af81e6e65c0}")},
            {"XboxComposite", new Guid("{05f5cfe2-4733-4950-a6bb-07aad01a3a84}")},
            {"XnaComposite", new Guid("{d61ca365-5af4-4486-998b-9db4734c6ca3}")}
        };

        public static List<string> ListDevices(Guid ClassGuid, Func<string[] /* DeviceID, DeviceDesc, FriendlyName, DeviceClass, DeviceDriver */, bool /* add to result? */> filter = null)
        {
            return ListDevices(ClassGuid.ToString(), filter);
        }

        public static List<string> ListDevices(string ClassGuid = null, Func<string[] /* DeviceID, DeviceDesc, FriendlyName, DeviceClass, DeviceDriver */, bool /* add to result? */> filter = null)
        {
            List<string> res = new List<string>();
            while (true)
            {
                try
                {
                    IntPtr hDevInfo = IntPtr.Zero;
                    if ((!string.IsNullOrEmpty(ClassGuid)) && (ClassGuid != Guid.Empty.ToString()))
                    {
                        Guid classGuid = DeviceClasses.ContainsKey(ClassGuid) ? DeviceClasses[ClassGuid] : new Guid(ClassGuid);
                        hDevInfo = SetupDiGetClassDevs(ref classGuid, null, IntPtr.Zero, DIGCF.DIGCF_PRESENT);
                    }
                    else
                    {
                        hDevInfo = SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, DIGCF.DIGCF_ALLCLASSES | DIGCF.DIGCF_DEVICEINTERFACE);
                    };                    
                    if (hDevInfo == (IntPtr)INVALID_HANDLE_VALUE) break;

                    SP_DEVINFO_DATA data = new SP_DEVINFO_DATA();
                    data.cbSize = Marshal.SizeOf(data);
                    for (int i = 0; SetupDiEnumDeviceInfo(hDevInfo, (uint)i, ref data); i++)
                    {
                        string DeviceID = GetDeviceID(data.DevInst);
                        if (filter != null)
                        {
                            string DeviceDesc   = GetStringPropertyForDevice(hDevInfo, data, (uint)SetupDiGetDeviceRegistryPropertyEnum.SPDRP_DEVICEDESC);
                            string FriendlyName = GetStringPropertyForDevice(hDevInfo, data, (uint)SetupDiGetDeviceRegistryPropertyEnum.SPDRP_FRIENDLYNAME);
                            string DeviceClass  = GetStringPropertyForDevice(hDevInfo, data, (uint)SetupDiGetDeviceRegistryPropertyEnum.SPDRP_CLASS);                                                        
                            string DeviceDriver = GetStringPropertyForDevice(hDevInfo, data, (uint)SetupDiGetDeviceRegistryPropertyEnum.SPDRP_DRIVER);
                            if (!filter(new string[] { DeviceID, DeviceDesc, FriendlyName, DeviceClass, DeviceDriver })) continue;
                        };
                        res.Add(DeviceID);
                    };

                    SetupDiDestroyDeviceInfoList(hDevInfo);
                    break;
                }
                catch { };
            };
            res.Sort();
            return res;
        }

        public static List<(string DeviceID, string Letter)> ListUSBStorages(bool onlyWithLetter = true)
        {
            List<(string DeviceID, string Letter)> res = new List<(string DeviceID, string Letter)>();
            foreach(string id in ListDevices(null, (list /* DeviceID, DeviceDesc, FriendlyName, DeviceClass, DeviceDriver */) => list[0].ToUpper().StartsWith("USBSTOR")))
            {
                DeviceInfo di = DeviceInfo.Get(id);
                string l = di.DriveLetter;
                if((!onlyWithLetter) || (!string.IsNullOrEmpty(l))) res.Add((id, l.Trim(new char[] { '\\',':' })));
                di.Dispose();
            };
            return res;
        }

        public static List<(string DeviceID, string Letter)> ListDiskStorages(bool onlyWithLetter = true)
        {
            List<(string DeviceID, string Letter)> res = new List<(string DeviceID, string Letter)>();

            foreach (string id in ListDevices(DeviceClasses["DiskDrive"]))
            {
                DeviceInfo di = DeviceInfo.Get(id);
                string l = di.DriveLetter;
                if ((!onlyWithLetter) || (!string.IsNullOrEmpty(l))) res.Add((id, l.Trim(new char[] { '\\', ':' })));
                di.Dispose();
            };            

            return res;
        }

        #endregion ListDevices
    }
}