# txt2Srt
Convert subtitles from *.txt text to *.srt after downloading the  Youtube videos. Just for learning English.
#转换的还不错，可以用。当mp4和srt文件同名时，potPlay会自动加载字幕。  2023.11.14
# 中文
## 基本思路：为了学习英语视频，把YouTube视频下载到本地，同时把自动翻译的英文文本也复制到本地。在播放的时候，需要把txt文本转换为SRT字幕文件。发现SRT文件的格式非常简单，而下载的txt文本中自带时间，只需要简单的转换就可以实现了。
### 针对以下几种情况，做了相应的开发工作。
#### 0 支持WIN7~WIN11，暂不支持MacOS和Linux系统。
#### 1、已经有了下载的文本文件，那么只需要指定此文件，程序应该自动转换为相应的SRT文件，保存在相应的路径。如果存在同名文件，自动覆盖。
#### 2、支持从网页中直接复制相应的文本（包含时间戳和内容），需要支持转换为SRT文件到指定位置。如果存在同名文件，自动覆盖。
#### 3、下一个版本，将支持中英文txt合并成一份字幕文件。
#English
