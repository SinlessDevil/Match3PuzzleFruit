# 🎨 SpriteOutlineGlow - Шейдер для Sprite Renderer

## 📋 Описание
Универсальный шейдер для Unity с обводкой, глоу эффектом и миганием для Sprite Renderer. Идеально подходит для игр типа Match3, где нужно выделять кусочки.

## ✨ Возможности

### 🎯 **Обводка (Outline)**
- ✅ Настраиваемый цвет обводки
- ✅ Регулируемая ширина обводки
- ✅ Настраиваемая прозрачность
- ✅ Пульсирующая обводка

### 🌟 **Глоу эффект**
- ✅ Настраиваемый цвет глоу
- ✅ Регулируемая интенсивность
- ✅ Настраиваемая мощность (power)
- ✅ Пульсирующий глоу

### ⚡ **Анимация**
- ✅ Мигание/пульсация
- ✅ Настраиваемая скорость
- ✅ Настраиваемая интенсивность
- ✅ Независимая пульсация для обводки и глоу

## 🚀 Установка

### 1. **Добавьте шейдер**
- Скопируйте `SpriteOutlineGlow.shader` в папку `Assets/Shaders/`
- Unity автоматически скомпилирует шейдер

### 2. **Создайте материал**
- Правый клик в Project → Create → Material
- Выберите шейдер `Custom/SpriteOutlineGlow`
- Настройте параметры

### 3. **Примените к спрайту**
- Перетащите материал на Sprite Renderer
- Или назначьте в компоненте Material

## ⚙️ Настройки шейдера

### **Основные параметры**
- **`_Color`** - Основной цвет спрайта
- **`_MainTex`** - Текстура спрайта

### **Обводка (Outline)**
- **`_OutlineColor`** - Цвет обводки
- **`_OutlineWidth`** - Ширина обводки (0-10)
- **`_OutlineAlpha`** - Прозрачность обводки (0-1)
- **`_EnableOutline`** - Включить/выключить обводку
- **`_OutlinePulse`** - Пульсация обводки

### **Глоу (Glow)**
- **`_GlowColor`** - Цвет глоу
- **`_GlowIntensity`** - Интенсивность глоу (0-5)
- **`_GlowPower`** - Мощность глоу (0.1-10)
- **`_GlowAlpha`** - Прозрачность глоу (0-1)
- **`_EnableGlow`** - Включить/выключить глоу
- **`_GlowPulse`** - Пульсация глоу

### **Пульсация (Pulse)**
- **`_PulseSpeed`** - Скорость пульсации (0-10)
- **`_PulseIntensity`** - Интенсивность пульсации (0-1)
- **`_EnablePulse`** - Включить/выключить пульсацию

## 🎮 Использование в коде

### **Базовое использование**
```csharp
// Получаем материал
Material material = spriteRenderer.material;

// Обводка
material.SetColor("_OutlineColor", Color.red);
material.SetFloat("_OutlineWidth", 2f);
material.SetFloat("_EnableOutline", 1f);

// Глоу
material.SetColor("_GlowColor", Color.blue);
material.SetFloat("_GlowIntensity", 2f);
material.SetFloat("_EnableGlow", 1f);

// Пульсация
material.SetFloat("_EnablePulse", 1f);
material.SetFloat("_PulseSpeed", 3f);
```

### **Использование с контроллером**
```csharp
// Добавьте SpriteOutlineGlowController на GameObject
SpriteOutlineGlowController controller = GetComponent<SpriteOutlineGlowController>();

// Быстрые настройки
controller.SetHighlightMode(true);    // Желтая обводка + глоу
controller.SetSelectedMode(true);     // Голубая обводка + глоу + пульсация

// Детальные настройки
controller.SetOutlineColor(Color.green);
controller.SetOutlineWidth(3f);
controller.SetGlowColor(Color.magenta);
controller.SetGlowIntensity(2.5f);
controller.EnableOutlinePulse();
```

## 🎯 Примеры для Match3 игры

### **1. Выделение при наведении**
```csharp
public void OnPieceHover()
{
    // Желтая обводка + глоу
    controller.SetHighlightMode(true);
}
```

### **2. Выбор кусочка**
```csharp
public void OnPiecePress()
{
    // Голубая обводка + глоу + пульсация
    controller.SetSelectedMode(true);
}
```

### **3. Совпадение кусочков**
```csharp
public void OnPieceMatch()
{
    // Зеленая обводка + глоу + быстрая пульсация
    controller.SetOutlineColor(Color.green);
    controller.SetGlowColor(Color.green);
    controller.SetOutlineWidth(4f);
    controller.SetGlowIntensity(3f);
    controller.EnableOutlinePulse();
    controller.EnableGlowPulse();
    controller.SetPulseSpeed(5f);
}
```

### **4. Особые кусочки**
```csharp
public void OnSpecialPiece()
{
    // Фиолетовая обводка + глоу + медленная пульсация
    controller.SetSpecialState(true);
}
```

## 🔧 Интеграция с MatchBoardController

### **Добавьте в GamePiece:**
```csharp
[RequireComponent(typeof(SpriteOutlineGlowController))]
public class GamePiece : MonoBehaviour
{
    private SpriteOutlineGlowController _visualController;
    
    private void Awake()
    {
        _visualController = GetComponent<SpriteOutlineGlowController>();
    }
    
    public void OnPress()
    {
        _visualController.OnPiecePress();
    }
    
    public void OnRelease()
    {
        _visualController.OnPieceRelease();
    }
    
    public void OnMatch()
    {
        _visualController.OnPieceMatch();
    }
}
```

## 📱 Оптимизация для мобильных устройств

### **Настройки производительности:**
- **Обводка**: `_OutlineWidth = 1-2` (меньше = быстрее)
- **Глоу**: `_GlowPower = 2-3` (больше = быстрее)
- **Пульсация**: Отключить на слабых устройствах

### **LOD система:**
```csharp
public void SetLowQualityMode(bool enabled)
{
    if (enabled)
    {
        controller.SetOutlineWidth(1f);
        controller.SetGlowPower(3f);
        controller.SetPulseEnabled(false);
    }
}
```

## 🎨 Цветовые схемы

### **Стандартные цвета:**
- **Выделение**: `Color.yellow` (#FFFF00)
- **Выбор**: `Color.cyan` (#00FFFF)
- **Совпадение**: `Color.green` (#00FF00)
- **Особые**: `Color.magenta` (#FF00FF)
- **Ошибка**: `Color.red` (#FF0000)

### **Кастомные цвета:**
```csharp
// Золотой
Color gold = new Color(1f, 0.84f, 0f);

// Серебряный
Color silver = new Color(0.75f, 0.75f, 0.75f);

// Бронзовый
Color bronze = new Color(0.8f, 0.5f, 0.2f);
```

## 🐛 Устранение неполадок

### **Шейдер не работает:**
1. Проверьте, что шейдер скомпилирован (нет ошибок в консоли)
2. Убедитесь, что материал назначен на Sprite Renderer
3. Проверьте, что спрайт имеет прозрачность (alpha channel)

### **Обводка не видна:**
1. Увеличьте `_OutlineWidth`
2. Проверьте `_OutlineAlpha` (должен быть > 0)
3. Убедитесь, что `_EnableOutline = 1`

### **Глоу слишком слабый:**
1. Увеличьте `_GlowIntensity`
2. Уменьшите `_GlowPower`
3. Проверьте `_GlowAlpha`

### **Пульсация не работает:**
1. Убедитесь, что `_EnablePulse = 1`
2. Проверьте `_PulseSpeed` (должен быть > 0)
3. Увеличьте `_PulseIntensity`

## 📚 Дополнительные ресурсы

### **Unity документация:**
- [ShaderLab](https://docs.unity3d.com/Manual/ShaderTut1.html)
- [Surface Shaders](https://docs.unity3d.com/Manual/SL-SurfaceShaders.html)
- [Material](https://docs.unity3d.com/ScriptReference/Material.html)

### **Лучшие практики:**
- Используйте LOD для разных устройств
- Кэшируйте ссылки на компоненты
- Отключайте эффекты на невидимых объектах
- Используйте Object Pooling для множественных эффектов

## 🎉 Результат
Теперь у вас есть мощный шейдер для создания красивых визуальных эффектов в вашей Match3 игре! 🚀

**Возможности:**
- ✅ Красивая обводка с настраиваемыми параметрами
- ✅ Эффектный глоу с регулируемой интенсивностью
- ✅ Плавная пульсация для привлечения внимания
- ✅ Простая интеграция с существующим кодом
- ✅ Оптимизация для мобильных устройств
