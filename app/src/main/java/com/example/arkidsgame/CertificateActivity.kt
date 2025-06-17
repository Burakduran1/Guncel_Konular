package com.example.arkidsgame

import android.content.Intent
import android.os.Bundle
import android.view.animation.AnimationUtils
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity

class CertificateActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_certificate)
        
        // UI elemanlarını tanımla
        val certificateImageView = findViewById<ImageView>(R.id.certificateImageView)
        val explorerNameTextView = findViewById<TextView>(R.id.explorerNameTextView)
        val startAdventureButton = findViewById<Button>(R.id.startAdventureButton)
        
        // Intent'ten kaşif adını al
        val explorerName = intent.getStringExtra("EXPLORER_NAME") ?: "Kaşif"
        
        // Kaşif adını göster
        explorerNameTextView.text = explorerName
        
        // Sertifika animasyonu
        val certificateAnimation = AnimationUtils.loadAnimation(this, R.anim.certificate_animation)
        certificateImageView.startAnimation(certificateAnimation)
        
        // Maceraya başla butonu
        startAdventureButton.setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            // Karakter özelliklerini aktar
            intent.putExtras(getIntent().extras ?: Bundle())
            startActivity(intent)
            finish()
        }
    }
} 