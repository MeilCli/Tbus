# Tbus
高槻キャンパスのバスの時刻表を表示できるようにする奴  
頑張れば市営バスすべての時刻表に対応できる  
[手抜きWebサイト](https://meilcli.github.io/Tbus/)

## データ
Github PagesにJsonとして置いてます。

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
|saturday_table||||Object|平日の時刻表|
||buses|||Array|バスの時間|
|||hour||int|時|
|||minute||int|分|
|||destination||string|系統・行先|
|sunday_table||||Object|平日の時刻表|
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


## ライセンス
under MIT License

using libraries:
- [AngleSharp](https://github.com/AngleSharp/AngleSharp)
- [Json.NET](https://github.com/JamesNK/Newtonsoft.Json)