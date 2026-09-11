# Bigger Capacity Fixed

基于 BiggerCapacity 开发，进行了一些修复和优化。

与原版 BiggerCapacity 不兼容，安装后请禁用原版mod。

## 注意

配置使用独立路径 `mods/settings/BiggerCapacityFixed/config.json`。

第一次加载本mod时将尝试从原版mod继承各项倍率和数值，调整数值可以在文件路径中进行或在游戏mod配置菜单中进行。

## 更改内容

### 游戏内配置界面

在游戏的模组管理页中，启用 Bigger Capacity Fixed 后，通过模组旁的“选项 / Options”按钮编辑配置。

基础容量区新增 `MiniFridge`（默认 50 kg）、`StorageTile`（默认 1000 kg）、`ConductionPanel`（默认 10 kg/s），上限均为 99999。Mini 冰箱和储存砖乘以 `StorageMultiplier`，导热板流量乘以 `PipeMultiplier`。储存砖的实际容量与可设置上限同步调整，玩家选择的较低容量仍然保留。配置版本升级为 8，已有配置会补入这三项默认值。此项改动尚未编译或安装。

### 发电机倍率修复

原 mod 放大 `Generator.WattageRating`，却没有放大发电机内部缓冲容量。以 800 W 发电机为例，原版 800 J 缓冲在每次 0.2 秒更新时会截断高倍率发电量，导致`GeneratorMultiplier`虽能调整大小，但实际上最大生效倍率仅为5。

### 变压器倍率修复和分离

从 `WireMultiplier` 分离变压器的控制倍数，并消除原 mod 会意外叠加发电机倍率的bug，现在变压器倍率 `TransformerMultiplier` 可以单独配置了。

### 迷你冰箱和储存砖修复

原 mod 中 迷你冰箱和储存砖走 `OtherMultiplier`，不受冰箱容量或储存倍率控制，现在将它们归类于储存倍率（与普通冰箱相同），并新增了单独的数值设置。

### 导热板修复

原 mod 中 导热板被忽略，现在走管道倍率，并新增数值设置。
