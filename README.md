# DNDriver 

Launch Metastock Downloader to start download process.

## Usage

- 顯示使用參數
```
C> DNDriver       
```

- 設定參數
```
C> DNDriver --server=http://127.0.0.1:4713 --path=C:\Metastock\Downloader\Downloader.exe --user=<downloader userid> --password=<downloader password>
```

## Installation

### Window Application Driver

- 安裝 Windows application driver
  - 下載路徑: https://github.com/microsoft/WinAppDriver/releases/download/v1.2.1/WindowsApplicationDriver_1.2.1.msi

- 設定 Windows application driver
  - Windows需開啟Developer模式(https://learn.microsoft.com/en-us/windows/apps/get-started/enable-your-device-for-development)

- 使用管理員權限執行Windows application driver, 預設會listen在http://127.0.0.1:4713

### Metastock Downloader

- 請將Metastock downloader安裝在C:\Metastock\Downloader底下,
- 下載的商品集請使用Legacy模式 (**非MSLocal**), 放在C:\Metastock\DataFiles底下,
- 請確認商品集的開始下載日期(First date)
- 先開啟Downloader, 設定帳號/密碼, 記住密碼(省得麻煩), 同時把商品集加到Download區裡面, 並且勾選每次要下載的商品集
- 手動下載, 先確認下載流程無誤,
- 關閉Downloader程式

### DNDriver

- 定時啟動DNDriver (請設定相關command line參數),
- DNDriver啟動時會自動叫起Downloader, 切換到Download tab, 點擊Download按鈕, 然後等到下載完成看到Report時, 關閉Report視窗, 以及Downloader程式,
- 如果一切成功的話, 程式的Exit code是0, 可以繼續後續動作,
- Log file放在Logs目錄內,
- 
  



