package com.example.arkidsgame

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.animation.AnimationUtils
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.cardview.widget.CardView

class MainActivity : AppCompatActivity() {

    private val TAG = "MainActivity"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        try {
            setContentView(R.layout.activity_main)

            // Kullanıcı adını al
            val explorerName = intent.getStringExtra("EXPLORER_NAME") ?: "Kaşif"

            // Seçilen karakter özelliklerini al ve kullan
            val clothes = intent.getIntExtra("CLOTHES", 0)
            val equipment = intent.getIntExtra("EQUIPMENT", 0)
            val vehicle = intent.getIntExtra("VEHICLE", 0)

            try {
                // Karşılama mesajını göster
                val welcomeTextView = findViewById<TextView>(R.id.welcomeTextView)
                welcomeTextView.text = "Hoş geldin, $explorerName!\nMaceraya hazır mısın?"

                // Karakter görselini göster
                val avatarImageView = findViewById<ImageView>(R.id.avatarImageView)
                avatarImageView.setImageResource(getAvatarImage(clothes))
                
                // Animasyon uygula
                val fadeIn = AnimationUtils.loadAnimation(this, R.anim.fade_in)
                avatarImageView.startAnimation(fadeIn)

                // Seçilen ekipmanı göster
                val equipmentImageView = findViewById<ImageView>(R.id.equipmentImageView)
                equipmentImageView.setImageResource(getEquipmentImage(equipment))
                equipmentImageView.startAnimation(fadeIn)

                // Seçilen aracı göster
                val vehicleImageView = findViewById<ImageView>(R.id.vehicleImageView)
                vehicleImageView.setImageResource(getVehicleImage(vehicle))
                vehicleImageView.startAnimation(fadeIn)
            } catch (e: Exception) {
                Log.e(TAG, "UI elemanlarını ayarlarken hata: ${e.message}", e)
                Toast.makeText(this, "Arayüz oluşturulurken hata: ${e.message}", Toast.LENGTH_LONG).show()
            }
            
            try {
                // Görev kartına tıklama işlevi
                val arMissionCard = findViewById<CardView>(R.id.arMissionCard)
                arMissionCard.setOnClickListener {
                    startARActivity()
                }
                
                // Oyna butonuna tıklama işlevi
                val playButton = findViewById<Button>(R.id.playButton)
                playButton.setOnClickListener {
                    startARActivity()
                }
                
                // Avatar kartına tıklama işlevi
                val avatarCard = findViewById<CardView>(R.id.avatarCard)
                avatarCard.setOnClickListener {
                    try {
                        // Karakter oluşturma ekranına git
                        val intent = Intent(this, CharacterCreationActivity::class.java)
                        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_NEW_TASK)
                        startActivity(intent)
                        finish()
                    } catch (e: Exception) {
                        Log.e(TAG, "Karakter oluşturma ekranına geçerken hata: ${e.message}", e)
                        Toast.makeText(this, "Karakter oluşturma ekranına geçerken hata: ${e.message}", Toast.LENGTH_LONG).show()
                    }
                }
                
                // Çıkış butonuna tıklama işlevi
                val exitButton = findViewById<Button>(R.id.exitButton)
                exitButton.setOnClickListener {
                    // Uygulamadan çık
                    finishAffinity()
                }
            } catch (e: Exception) {
                Log.e(TAG, "Buton işlevlerini ayarlarken hata: ${e.message}", e)
                Toast.makeText(this, "Buton işlevlerini ayarlarken hata: ${e.message}", Toast.LENGTH_LONG).show()
            }
        } catch (e: Exception) {
            Log.e(TAG, "onCreate hatası: ${e.message}", e)
            Toast.makeText(this, "Uygulama başlatılırken hata oluştu: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }
    
    private fun startARActivity() {
        // AR aktivitesine geçiş
        try {
            val intent = Intent(this, ARActivity::class.java)
            startActivity(intent)
        } catch (e: Exception) {
            Log.e(TAG, "AR aktivitesine geçerken hata: ${e.message}", e)
            Toast.makeText(this, "AR aktivitesine geçerken hata: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }

    private fun getAvatarImage(clothes: Int): Int {
        return when (clothes) {
            0 -> R.drawable.avatar_clothes_1
            1 -> R.drawable.avatar_clothes_2
            2 -> R.drawable.avatar_clothes_3
            else -> R.drawable.avatar_clothes_1
        }
    }

    private fun getEquipmentImage(equipment: Int): Int {
        return when (equipment) {
            0 -> R.drawable.equipment_binoculars
            1 -> R.drawable.equipment_compass
            2 -> R.drawable.equipment_notebook
            3 -> R.drawable.equipment_camera
            else -> R.drawable.equipment_binoculars
        }
    }

    private fun getVehicleImage(vehicle: Int): Int {
        return when (vehicle) {
            0 -> R.drawable.vehicle_magic_carpet
            1 -> R.drawable.vehicle_airplane
            2 -> R.drawable.vehicle_rocket
            3 -> R.drawable.vehicle_balloon
            else -> R.drawable.vehicle_magic_carpet
        }
    }
}