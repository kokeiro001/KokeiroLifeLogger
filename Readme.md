# KokeiroLifeLogger

## 概要

自分用のオレオレライフロガー。

AzureFunctionsを中心に、IFTTTとかGoogleAnalyticsだとかと連携して、日々の情報を集める。


## 機能

### いいねしたツイート、GitHubでスターを付けたリポジトリ、Pocketで保存した記事を記録する機能

IFTTTと連携してAzure Table Storageに記録する機能。
IFTTTのifで各サービスを、thenでWebhooksを選択し、JSONを```IFTTTHttpTrigger```に送りつけることで実現している。

TwitterとPocketの連携は元から用意されているサービスを用いて実現できる。

GitHubでスター付けたイベントはGitHubの各個人atomフィードから取得できる。

例)https://github.com/kokeiro001.atom


### 特定の場所を出入りしたことを記録する機能

IFTTTと連携してAzure Table Storageに記録する機能。
IFTTTのifでLocationサービスを、thenでWebhooksを選択し、JSONを```LocationEnteredOrExitedHttpTrigger```に送りつけることで実現している。


### ニコニコのマイリストに登録されてる動画情報を記録する機能

定期的に再生数、マイリスト数、コメント数を記憶する機能。```NicoNicoMyListObserver```のタイマートリガーで実現している。

新しい動画を投稿することがどれくらい過去の動画に影響するかとか知りたかったので。

愚直にマイリストページを開き、取得したHTMLをパースしている。


### 日々のライフログをmixiに日記投稿する機能

Azure Table Storageに蓄えた一日分の情報をmixiに日記投稿する機能。```PostDiary2Mixics```のタイマートリガーで実現している。

mixiはメールで日記投稿できるのでそれを利用している。メールはGmailから送信している。


### 体重や体脂肪率を記録する機能

Withingsの体重計とIFTTTを連携させて、体重や体脂肪率を記録する機能。

IFTTTのifでWithingsサービスを、thenでWebhooksを選択し、JSONを```WeightMeasurementTrigger```に送りつけることで実現している。


### 起床時間、就寝時間を記録する機能

Withings SleepとIFTTTを連携させて、起床時間、就寝時間を記録する機能。

IFTTTのifでWithings　Sleepサービスを、thenでWebhooksを選択し、JSONを```WeightMeasurementTrigger```に送りつけることで実現している。



## 使い方

そのうち書く。。
様々な外部サービスと連携しているため、それらサービス先でも多様な設定が必要。


