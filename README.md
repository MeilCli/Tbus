# Tbus
高槻キャンパスのバスの時刻表を表示できるようにする奴  
頑張れば市営バスすべての時刻表に対応できる  
[手抜きWebサイト](https://meilcli.github.io/Tbus/)と手抜きアプリ(Android, iOS, UWP)を用意してるのでビルドして遊んでみてください

## データ
Github PagesにJsonとして置いてます。
### 時刻表
URLs
- [https://github.com/MeilCli/Tbus/tree/master/docs/timetable](https://github.com/MeilCli/Tbus/tree/master/docs/timetable)
- https://meilcli.github.io/Tbus/timetable/
  - Raw Data
  - 実際にアクセスする際はファイル名もURLに含めてください

データの形はテーブルにしてますが、以下の2つのファイルを見てもわかると思います。  
- [kansai_takatuki.json](docs/timetable/kansai_takatuki.json)
- [kansai_takatuki.limited2.json](docs/timetable/kansai_takatuki.limited2.json)

|Key||||Type|Value|
|:--:|:--:|:--:|:--:|:--:|:--:|
|id||||string|時刻表の識別子(期間限定ファイルで重複の可能性あり)|
|created_at||||DateTime|ファイル作成日時|
|limited_time_option||||Object|限定期間|
||start_day|||DateTime|限定期間の初日|
||end_day|||DateTime|限定期間の最終日|
|weekday_table||||Object|平日の時刻表|
||buses|||Array|バスの時間|
|||hour||int|時|
|||minute||int|分|
|||destination||string|系統・行先|
|saturday_table||||Object|土曜の時刻表|
||buses|||Array|バスの時間|
|||hour||int|時|
|||minute||int|分|
|||destination||string|系統・行先|
|sunday_table||||Object|日曜の時刻表|
||buses|||Array|バスの時間|
|||hour||int|時|
|||minute||int|分|
|||destination||string|系統・行先|
|special_days||||Dictionary|限定時刻表|
||{{DateTime}}|||Object|Keyは日付、Valueはその日の時刻表|
|||buses||Array|バスの時間|
||||hour|int|時|
||||minute|int|分|
||||destination|string|系統・行先|
|hour_table||||Object|時間の順序(深夜0時などを並べるのに必要)|
||indexed_hours|||Array|順序付き時|
|||index||int|順番|
|||hour||int|時|

### カレンダー
URLs
- [https://github.com/MeilCli/Tbus/tree/master/docs/calendar](https://github.com/MeilCli/Tbus/tree/master/docs/calendar)
- https://meilcli.github.io/Tbus/calendar/
  - Raw Data
  - 実際にアクセスする際はファイル名もURLに含めてください

データの形はテーブルにしてますが、以下のファイルを見たほうがわかると思います。  
- [2019.json](docs/calendar/2019.json)

|Key||Type|Value|
|:--:|:--:|:--:|:--:|
|||Array|その年のカレンダー|
||date|DateTime|日付|
||day_type|int|0 = 平日, 1 = 土曜日, 2 = 日曜日, 3 = 祝日|

### 祝日
URLs
- [https://github.com/MeilCli/Tbus/tree/master/docs/holiday](https://github.com/MeilCli/Tbus/tree/master/docs/holiday)
- https://meilcli.github.io/Tbus/holiday/
  - Raw Data
  - 実際にアクセスする際はファイル名もURLに含めてください

データの形はテーブルにしてますが、以下のファイルを見たほうがわかると思います。  
- [2019.json](docs/holiday/2019.json)

|Type|Value|
|:--:|:--:|
|DateTime[]|祝日の日付|

## ビルド
必要な環境
- Visual Studio 2017
- C# 7.3
- TypeScript 2.9

### Tbus.App.XamarinForms.Android
Androidアプリケーション

ビルドに必要な環境
- Xamarin.Android

実行に必要な環境
- Android EmulatorまたはAndroid実機

### Tbus.App.XamarinForms.iOS
iOSアプリケーション

ビルドに必要な環境
- Xamarin.iOS

実行に必要な環境
- iOS EmulatorまたはiOS実機

### Tbus.App.XamarinForms.UWP
UWPアプリケーション

ビルドに必要な環境
- UWP SDK

実行に必要な環境
- Windows 10 mobile EmulatorまたはWindows 10搭載端末

### Tbus.Calendar.NETCore.Condole
カレンダー・祝日生成コンソールアプリケーション

実行に必要な環境
- .NET Core 2.0

### Tbus.Parser.NETCore.Console
時刻表生成コンソールアプリケーション  

実行に必要な環境
- .NET Core 2.0

### Tbus.Web
時刻表を見れるWebサイト

実行に必要な環境
- モダンなブラウザ(Chromeとか)

## ライセンス
under MIT License

using libraries:
- [AngleSharp](https://github.com/AngleSharp/AngleSharp)
- [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)
- [ReactiveProperty](https://github.com/runceel/ReactiveProperty)
- [Autofac](https://github.com/autofac/Autofac)
- [Xamarin.Forms.BehaviorsPack](https://github.com/nuitsjp/Xamarin.Forms.BehaviorsPack)