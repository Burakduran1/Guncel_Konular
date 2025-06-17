package com.example.arkidsgame

import android.Manifest
import android.content.pm.PackageManager
import android.media.MediaPlayer
import android.os.Bundle
import android.os.CountDownTimer
import android.util.Log
import android.view.View
import android.view.animation.AnimationUtils
import android.widget.Button
import android.widget.FrameLayout
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.camera.core.CameraSelector
import androidx.camera.core.Preview
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.camera.view.PreviewView
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors
import kotlin.random.Random

class ARActivity : AppCompatActivity() {
    
    private val TAG = "ARActivity"
    private val CAMERA_PERMISSION_CODE = 1001
    
    private lateinit var arImageView: ImageView
    private lateinit var infoTextView: TextView
    private lateinit var backButton: Button
    private lateinit var startGameButton: Button
    private lateinit var scoreTextView: TextView
    private lateinit var timerTextView: TextView
    // arFragmentContainer artık gerekli değil
    private lateinit var arControlsLayout: LinearLayout
    private lateinit var titleTextView: TextView
    private lateinit var gameStatusLayout: LinearLayout
    private lateinit var buttonLayout: LinearLayout
    private lateinit var contentFrame: FrameLayout
    private lateinit var arBackButton: Button
    
    // AR kontrol butonları
    private lateinit var hairStyleButton: ImageButton
    private lateinit var eyeColorButton: ImageButton
    private lateinit var clothesButton: ImageButton
    private lateinit var accessoryButton: ImageButton
    private lateinit var equipmentButton: ImageButton
    private lateinit var vehicleButton: ImageButton
    
    private var score = 0
    private var gameActive = false
    private var gameTime = 30 // saniye
    
    private lateinit var countDownTimer: CountDownTimer
    private lateinit var clickSound: MediaPlayer
    private lateinit var successSound: MediaPlayer
    
    // AR değişkenleri - artık gerekli değil (CameraX kullanıyoruz)
    
    // Aktif karakter özellikleri
    private var currentHairStyle = 0
    private var currentEyeColor = 0
    private var currentClothes = 0
    private var currentAccessory = 0
    private var currentEquipment = 0
    private var currentVehicle = 0
    
    // Mevcut özellik resimleri
    private val hairStyles = listOf(
        R.drawable.avatar_hair_1, 
        R.drawable.avatar_hair_2, 
        R.drawable.avatar_hair_3
    )
    
    private val eyeColors = listOf(
        R.drawable.avatar_eye_1, 
        R.drawable.avatar_eye_2, 
        R.drawable.avatar_eye_3
    )
    
    private val clothes = listOf(
        R.drawable.avatar_clothes_1, 
        R.drawable.avatar_clothes_2, 
        R.drawable.avatar_clothes_3
    )
    
    private val accessories = listOf(
        R.drawable.avatar_accessory_1, 
        R.drawable.avatar_accessory_2, 
        R.drawable.avatar_accessory_3
    )
    
    private val equipment = listOf(
        R.drawable.equipment_binoculars,
        R.drawable.equipment_compass,
        R.drawable.equipment_notebook,
        R.drawable.equipment_camera
    )
    
    private val vehicles = listOf(
        R.drawable.vehicle_magic_carpet,
        R.drawable.vehicle_airplane,
        R.drawable.vehicle_rocket,
        R.drawable.vehicle_balloon
    )

    private lateinit var previewView: PreviewView
    private lateinit var overlayView: OverlayView
    private lateinit var cameraExecutor: ExecutorService

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_ar)
        
        // UI elemanlarını tanımla
        arImageView = findViewById(R.id.arImageView)
        infoTextView = findViewById(R.id.infoTextView)
        backButton = findViewById(R.id.backButton)
        startGameButton = findViewById(R.id.startGameButton)
        scoreTextView = findViewById(R.id.scoreTextView)
        timerTextView = findViewById(R.id.timerTextView)
        // arFragmentContainer kaldırıldı - artık CameraX kullanıyoruz
        arControlsLayout = findViewById(R.id.arControlsLayout)
        titleTextView = findViewById(R.id.titleTextView)
        gameStatusLayout = findViewById(R.id.gameStatusLayout)
        buttonLayout = findViewById(R.id.buttonLayout)
        contentFrame = findViewById(R.id.contentFrame)
        arBackButton = findViewById(R.id.arBackButton)
        
        // AR kontrol butonlarını tanımla
        hairStyleButton = findViewById(R.id.hairStyleButton)
        eyeColorButton = findViewById(R.id.eyeColorButton)
        clothesButton = findViewById(R.id.clothesButton)
        accessoryButton = findViewById(R.id.accessoryButton)
        equipmentButton = findViewById(R.id.equipmentButton)
        vehicleButton = findViewById(R.id.vehicleButton)
        
        // Karakter oluşturma ekranından gelen verileri al
        getCharacterData()
        
        // Ses efektlerini yükle
        loadSounds()
        
        // AR kontrollerini ayarla
        setupARControls()
        
        previewView = findViewById(R.id.previewView)
        overlayView = findViewById(R.id.overlayView)
        
        // Başlangıçta AR view'ları gizle
        previewView.visibility = View.GONE
        overlayView.visibility = View.GONE  // Overlay sürekli gizli kalacak
        
        // AR kontrol butonlarını göster
        arControlsLayout.visibility = View.VISIBLE
        
        // AR modunu başlat butonu ekle
        startGameButton.visibility = View.VISIBLE
        startGameButton.text = "AR Modunu Başlat"
        startGameButton.setOnClickListener {
            startARMode()
        }
        
        // Geri butonu
        backButton.setOnClickListener {
            if (gameActive) {
                // Oyun aktifse, onaylamak için sor
                showExitConfirmation()
            } else {
                finish()
            }
        }
        
        // AR modu geri butonu
        arBackButton.setOnClickListener {
            // AR modundan çık
            exitARMode()
        }
    }
    
    private fun getCharacterData() {
        // Karakter oluşturma ekranından gelen intent verilerini al
        try {
            intent?.let { intent ->
                currentHairStyle = intent.getIntExtra("HAIR_STYLE", 0)
                currentEyeColor = intent.getIntExtra("EYE_COLOR", 0)
                currentClothes = intent.getIntExtra("CLOTHES", 0)
                currentAccessory = intent.getIntExtra("ACCESSORY", 0)
                currentEquipment = intent.getIntExtra("EQUIPMENT", 0)
                currentVehicle = intent.getIntExtra("VEHICLE", 0)
                
                // Otomatik AR başlatma
                val startARMode = intent.getBooleanExtra("START_AR_MODE", false)
                if (startARMode) {
                    // AR modu otomatik başlatılsın mı?
                    Log.i(TAG, "Otomatik AR modu başlatılacak")
                }
            }
        } catch (e: Exception) {
            Log.e(TAG, "Karakter verilerini alırken hata: ${e.message}")
        }
    }
    
    private fun setupARControls() {
        // Saç stili butonuna tıklama
        hairStyleButton.setOnClickListener {
            currentHairStyle = (currentHairStyle + 1) % hairStyles.size
            hairStyleButton.setBackgroundResource(hairStyles[currentHairStyle])
            updateARFaceModel()
        }
        
        // Göz rengi butonuna tıklama
        eyeColorButton.setOnClickListener {
            currentEyeColor = (currentEyeColor + 1) % eyeColors.size
            eyeColorButton.setBackgroundResource(eyeColors[currentEyeColor])
            updateARFaceModel()
        }
        
        // Kıyafet butonuna tıklama
        clothesButton.setOnClickListener {
            currentClothes = (currentClothes + 1) % clothes.size
            clothesButton.setBackgroundResource(clothes[currentClothes])
            updateARFaceModel()
        }
        
        // Aksesuar butonuna tıklama
        accessoryButton.setOnClickListener {
            currentAccessory = (currentAccessory + 1) % accessories.size
            accessoryButton.setBackgroundResource(accessories[currentAccessory])
            updateARFaceModel()
        }
        
        // Ekipman butonuna tıklama
        equipmentButton.setOnClickListener {
            currentEquipment = (currentEquipment + 1) % equipment.size
            equipmentButton.setBackgroundResource(equipment[currentEquipment])
            updateARFaceModel()
        }
        
        // Araç butonuna tıklama
        vehicleButton.setOnClickListener {
            currentVehicle = (currentVehicle + 1) % vehicles.size
            vehicleButton.setBackgroundResource(vehicles[currentVehicle])
            updateARFaceModel()
        }
    }
    
    private fun updateARFaceModel() {
        // OverlayView'ı güncelle (butonlara basıldığında)
        try {
            overlayView.setCharacterFeatures(
                hairRes = hairStyles[currentHairStyle],
                eyeRes = eyeColors[currentEyeColor],
                clothesRes = clothes[currentClothes],
                accessoryRes = accessories[currentAccessory],
                equipmentRes = equipment[currentEquipment],
                vehicleRes = vehicles[currentVehicle]
            )
            Log.d(TAG, "AR model güncellendi - Hair: $currentHairStyle, Eye: $currentEyeColor, Clothes: $currentClothes")
        } catch (e: Exception) {
            Log.e(TAG, "AR model güncellenemedi: ${e.message}", e)
        }
    }
    
    // checkPermissions artık gerekli değil - startARMode'da yapıyoruz

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == CAMERA_PERMISSION_CODE && grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            startCamera()
        } else {
            Toast.makeText(this, "Kamera izni olmadan AR modu kullanılamaz", Toast.LENGTH_SHORT).show()
            // AR modundan çık
            exitARMode()
        }
    }
    
    // initializeAR ve setupAR artık gerekli değil - direkt startARMode() çağırıyoruz
    
    private fun startARMode() {
        // Activity destroy kontrolü
        if (isDestroyed || isFinishing) {
            Log.w(TAG, "Activity destroy olmuş, AR başlatılamıyor")
            return
        }
        
        Log.d(TAG, "AR modu başlatılıyor...")
        
        // AR başlatma butonunu gizle
        startGameButton.visibility = View.GONE
        
        // ARCore ve fragment ile ilgili kodları kaldırdık, CameraX ile kamera açıyoruz
        previewView.visibility = View.VISIBLE
        overlayView.visibility = View.VISIBLE  // Overlay'ı aktifleştir - karakter özelliklerini göster
        arImageView.visibility = View.GONE
        
        // AR modunda diğer UI elemanlarını gizle (tam ekran kamera için)
        titleTextView.visibility = View.GONE
        infoTextView.visibility = View.GONE
        gameStatusLayout.visibility = View.GONE
        contentFrame.visibility = View.GONE
        startGameButton.visibility = View.GONE  // AR başlatma butonunu gizle
        buttonLayout.visibility = View.GONE     // Ana buton layout'unu gizle
        arBackButton.visibility = View.VISIBLE  // AR geri butonunu göster
        arControlsLayout.visibility = View.VISIBLE  // AR kontrol butonlarını göster
        
        // Kamera izni kontrolü (sadece bir kez)
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) == PackageManager.PERMISSION_GRANTED) {
            Log.d(TAG, "Kamera izni var, kamera başlatılıyor")
            startCamera()
        } else {
            Log.d(TAG, "Kamera izni yok, izin isteniyor")
            ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.CAMERA), CAMERA_PERMISSION_CODE)
        }
        
        // OverlayView'a karakter özelliklerini aktarır (2D çizim)
        try {
            overlayView.setCharacterFeatures(
                hairRes = hairStyles[currentHairStyle],
                eyeRes = eyeColors[currentEyeColor],
                clothesRes = clothes[currentClothes],
                accessoryRes = accessories[currentAccessory],
                equipmentRes = equipment[currentEquipment],
                vehicleRes = vehicles[currentVehicle]
            )
            Log.d(TAG, "Karakter özellikleri overlay'e aktarıldı")
        } catch (e: Exception) {
            Log.e(TAG, "Karakter özellikleri aktarılamadı: ${e.message}", e)
        }
    }
    
    private fun exitARMode() {
        Log.d(TAG, "AR modundan çıkılıyor...")
        
        // Kamera kaynakları temizle
        cleanupAR()
        
        // Normal moda dön
        setupInitialState()
    }

    private fun startCamera() {
        try {
            val cameraProviderFuture = ProcessCameraProvider.getInstance(this)
            cameraProviderFuture.addListener({
                try {
                    val cameraProvider = cameraProviderFuture.get()
                    val preview = Preview.Builder().build().also {
                        it.setSurfaceProvider(previewView.surfaceProvider)
                    }
                    val cameraSelector = CameraSelector.DEFAULT_FRONT_CAMERA
                    cameraProvider.unbindAll()
                    cameraProvider.bindToLifecycle(this, cameraSelector, preview)
                    Log.d(TAG, "Kamera başarıyla başlatıldı")
                } catch (e: Exception) {
                    Log.e(TAG, "Kamera başlatılamadı: ${e.message}", e)
                    // AR modundan çık
                    exitARMode()
                }
            }, ContextCompat.getMainExecutor(this))
            cameraExecutor = Executors.newSingleThreadExecutor()
        } catch (e: Exception) {
            Log.e(TAG, "CameraX başlatılamadı: ${e.message}", e)
            exitARMode()
        }
    }
    
    private fun loadSounds() {
        try {
            // Ses kaynakları henüz oluşturulmadığında hata oluşmaması için
            try {
                // Tıklama sesi - raw klasörü henüz boş
                clickSound = MediaPlayer.create(this, R.raw.catchh)
            } catch (e: Exception) {
                // Ses dosyası yoksa MediaPlayer oluştur ve sessiz yap
                clickSound = MediaPlayer()
            }

            try {
                // Başarı sesi - raw klasörü henüz boş
                successSound = MediaPlayer.create(this, R.raw.match)
            } catch (e: Exception) {
                // Ses dosyası yoksa MediaPlayer oluştur ve sessiz yap
                successSound = MediaPlayer()
            }
        } catch (e: Exception) {
            // Ses yüklenemezse hata mesajı göster
            Log.e(TAG, "Ses efektleri yüklenemedi: ${e.message}", e)
            // Toast kaldırıldı - çok fazla toast sorunu yaratıyor
        }
    }
    
    private fun setupInitialState() {
        // AR modundan çıkışta tüm UI elemanlarını geri göster
        titleTextView.visibility = View.VISIBLE
        infoTextView.visibility = View.VISIBLE
        gameStatusLayout.visibility = View.VISIBLE
        contentFrame.visibility = View.VISIBLE
        buttonLayout.visibility = View.VISIBLE
        
        // Kamera ve overlay'ı gizle
        previewView.visibility = View.GONE
        overlayView.visibility = View.GONE
        arBackButton.visibility = View.GONE
        arControlsLayout.visibility = View.GONE
        
        infoTextView.text = "Artırılmış Gerçeklik modüllerimiz henüz tam olarak hazır değil, " +
                "ama mini bir oyun oynayabilirsin!"
        startGameButton.visibility = View.VISIBLE
        startGameButton.setText("Oyunu Başlat")
        scoreTextView.visibility = View.INVISIBLE
        timerTextView.visibility = View.INVISIBLE
        // arFragmentContainer artık yok - CameraX kullanıyoruz
        arControlsLayout.visibility = View.GONE
        gameActive = false
        score = 0
        
        // Sertifika şablonunu göster
        arImageView.setImageResource(R.drawable.certificate_template)
        arImageView.visibility = View.VISIBLE
        
        // Animasyon uygula
        val fadeIn = AnimationUtils.loadAnimation(this, R.anim.fade_in)
        arImageView.startAnimation(fadeIn)
        
        // Oyunu başlat butonu
        startGameButton.setOnClickListener {
            startGame()
        }
        
        // Görsel tıklama
        arImageView.setOnClickListener {
            if (gameActive) {
                handleImageClick()
            }
        }
    }

    private fun startGame() {
        gameActive = true
        score = 0
        startGameButton.visibility = View.GONE
        scoreTextView.visibility = View.VISIBLE
        timerTextView.visibility = View.VISIBLE
        
        // Skoru sıfırla ve göster
        updateScore()
        
        // Geri sayımı başlat
        startCountdown()
        
        // İlk görüntüyü ayarla
        setRandomImage()
    }
    
    private fun startCountdown() {
        // Önceki sayacı iptal et
        if (::countDownTimer.isInitialized) {
            countDownTimer.cancel()
        }
        
        // Yeni sayaç oluştur
        countDownTimer = object : CountDownTimer((gameTime * 1000).toLong(), 1000) {
            override fun onTick(millisUntilFinished: Long) {
                val secondsLeft = millisUntilFinished / 1000
                timerTextView.text = "Süre: $secondsLeft sn"
            }
            
            override fun onFinish() {
                endGame()
            }
        }.start()
    }
    
    private fun handleImageClick() {
        // Skoru artır
        score++
        updateScore()
        
        // Tıklama sesi çal
        playClickSound()
        
        // Tıklama animasyonu
        val clickAnim = AnimationUtils.loadAnimation(this, R.anim.certificate_animation)
        arImageView.startAnimation(clickAnim)
        
        // Yeni rastgele görüntü
        setRandomImage()
    }

    private fun updateScore() {
        scoreTextView.text = "Skor: $score"
    }
    
    private fun playClickSound() {
        try {
            if (clickSound.isPlaying) {
                clickSound.seekTo(0)
            } else {
                clickSound.start()
            }
        } catch (e: Exception) {
            // Ses çalınamazsa sessizce devam et
        }
    }
    
    private fun playSuccessSound() {
        try {
            successSound.start()
        } catch (e: Exception) {
            // Ses çalınamazsa sessizce devam et
        }
    }
    
    private fun setRandomImage() {
        // Rastgele bir resim seç
        val images = listOf(
            R.drawable.avatar_clothes_1,
            R.drawable.avatar_clothes_2,
            R.drawable.avatar_clothes_3,
            R.drawable.equipment_binoculars,
            R.drawable.equipment_compass,
            R.drawable.equipment_notebook,
            R.drawable.equipment_camera,
            R.drawable.vehicle_magic_carpet,
            R.drawable.vehicle_airplane,
            R.drawable.vehicle_rocket,
            R.drawable.vehicle_balloon
        )
        
        val randomImage = images[Random.nextInt(images.size)]
        arImageView.setImageResource(randomImage)
    }
    
    private fun endGame() {
        // Oyunu bitir
        gameActive = false
        
        // Sayacı durdur
        if (::countDownTimer.isInitialized) {
            countDownTimer.cancel()
        }
        
        // Başarı sesi çal
        playSuccessSound()
        
        // Sonuç mesajı
        val message = when {
            score >= 20 -> "Muhteşem! $score puan topladın!"
            score >= 15 -> "Harika! $score puan topladın!"
            score >= 10 -> "İyi iş! $score puan topladın!"
            else -> "Tebrikler! $score puan topladın!"
        }
        
        Toast.makeText(this, message, Toast.LENGTH_LONG).show()
        
        // Oyunu sıfırla
        setupInitialState()
    }
    
    private fun showExitConfirmation() {
        Toast.makeText(
            this,
            "Oyundan çıkmak için tekrar GERİ tuşuna basın",
            Toast.LENGTH_SHORT
        ).show()
        
        // İkinci basışta çıkış
        backButton.setOnClickListener {
            // Sayacı durdur
            if (::countDownTimer.isInitialized) {
                countDownTimer.cancel()
            }
            
            // Sesleri temizle
            releaseMediaPlayers()
            
            // AR kaynakları temizle
            cleanupAR()
            
            finish()
        }
    }
    
    private fun cleanupAR() {
        try {
            // CameraX kaynakları temizle
            if (::cameraExecutor.isInitialized) {
                cameraExecutor.shutdown()
            }
        } catch (e: Exception) {
            Log.e(TAG, "AR kaynakları temizlenirken hata: ${e.message}", e)
        }
    }
    
    private fun releaseMediaPlayers() {
        try {
            if (::clickSound.isInitialized) {
                clickSound.release()
            }
            if (::successSound.isInitialized) {
                successSound.release()
            }
        } catch (e: Exception) {
            // Sessizce devam et
        }
    }
    
    override fun onResume() {
        super.onResume()
    }
    
    override fun onPause() {
        super.onPause()
    }

    override fun onDestroy() {
        super.onDestroy()
        
        // Sayacı durdur
        if (::countDownTimer.isInitialized) {
            countDownTimer.cancel()
        }
        
        // MediaPlayer kaynaklarını serbest bırak
        releaseMediaPlayers()
        
        // AR kaynakları temizle
        cleanupAR()
        
        if (::cameraExecutor.isInitialized) {
            cameraExecutor.shutdown()
        }
    }
} 