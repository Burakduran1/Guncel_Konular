package com.example.arkidsgame

import android.content.Context
import android.graphics.BitmapFactory
import android.graphics.Canvas
import android.graphics.Paint
import android.util.AttributeSet
import android.view.View
import androidx.core.content.ContextCompat

class OverlayView @JvmOverloads constructor(
    context: Context, 
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : View(context, attrs, defStyleAttr) {
    private var hairRes: Int? = null
    private var eyeRes: Int? = null
    private var clothesRes: Int? = null
    private var accessoryRes: Int? = null
    private var equipmentRes: Int? = null
    private var vehicleRes: Int? = null

    fun setCharacterFeatures(
        hairRes: Int?,
        eyeRes: Int?,
        clothesRes: Int?,
        accessoryRes: Int?,
        equipmentRes: Int?,
        vehicleRes: Int?
    ) {
        this.hairRes = hairRes
        this.eyeRes = eyeRes
        this.clothesRes = clothesRes
        this.accessoryRes = accessoryRes
        this.equipmentRes = equipmentRes
        this.vehicleRes = vehicleRes
        invalidate()
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        
        // Boyut kontrolü
        if (width <= 0 || height <= 0) return
        
        val paint = Paint()
        paint.isAntiAlias = true
        
        // Basitçe üst üste bindir: ortada, küçük boyutlarda
        val centerX = width / 2f
        val centerY = height / 2f
        val size = 100 // Sabit boyut kullan
        val offset = size / 2

        // Güvenli bitmap çizimi
        drawSafeBitmap(canvas, hairRes, centerX - size/2, centerY - size, paint)
        drawSafeBitmap(canvas, eyeRes, centerX - size/4, centerY - size/4, paint)
        drawSafeBitmap(canvas, clothesRes, centerX - size/2, centerY + offset, paint)
        drawSafeBitmap(canvas, accessoryRes, centerX + offset, centerY - size/2, paint)
        drawSafeBitmap(canvas, equipmentRes, centerX - size, centerY + offset, paint)
        drawSafeBitmap(canvas, vehicleRes, centerX - size/2, centerY + size, paint)
    }
    
    private fun drawSafeBitmap(canvas: Canvas, resId: Int?, x: Float, y: Float, paint: Paint) {
        resId?.let {
            try {
                val options = BitmapFactory.Options()
                options.inSampleSize = 4 // Bitmap'i küçült
                val bmp = BitmapFactory.decodeResource(resources, it, options)
                bmp?.let { bitmap ->
                    if (!bitmap.isRecycled && bitmap.width > 0 && bitmap.height > 0) {
                        // Bitmap'i sabit boyuta ölçekle
                        val scaledBitmap = android.graphics.Bitmap.createScaledBitmap(bitmap, 64, 64, true)
                        canvas.drawBitmap(scaledBitmap, x, y, paint)
                        // Bellek sızıntısını önle
                        if (scaledBitmap != bitmap) {
                            scaledBitmap.recycle()
                        }
                    }
                }
            } catch (e: Exception) {
                // Bitmap yüklenemezse sessizce devam et
            }
        }
    }
} 