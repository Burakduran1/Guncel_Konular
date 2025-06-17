package com.example.arkidsgame

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.View
import android.view.animation.AnimationUtils
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity

class WelcomeActivity : AppCompatActivity() {

    private lateinit var titleTextView: TextView
    private lateinit var logoImageView: ImageView
    private lateinit var startButton: Button
    private lateinit var aboutButton: Button
    
    private val TAG = "WelcomeActivity"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        try {
            setContentView(R.layout.activity_welcome)

            // UI elemanlarını tanımla
            titleTextView = findViewById(R.id.titleTextView)
            logoImageView = findViewById(R.id.logoImageView)
            startButton = findViewById(R.id.startButton)
            aboutButton = findViewById(R.id.aboutButton)

            // Animasyonları yükle
            val fadeIn = AnimationUtils.loadAnimation(this, R.anim.fade_in)
            val slideUp = AnimationUtils.loadAnimation(this, R.anim.slide_up)

            // Animasyonları uygula
            titleTextView.startAnimation(fadeIn)
            logoImageView.startAnimation(fadeIn)
            startButton.startAnimation(slideUp)
            aboutButton.startAnimation(slideUp)

            // Başla butonuna tıklama işlevi
            startButton.setOnClickListener {
                try {
                    val intent = Intent(this, CharacterCreationActivity::class.java)
                    intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_NEW_TASK)
                    startActivity(intent)
                } catch (e: Exception) {
                    Log.e(TAG, "Karakter oluşturma ekranına geçerken hata: ${e.message}", e)
                    Toast.makeText(this, "Bir hata oluştu: ${e.message}", Toast.LENGTH_LONG).show()
                }
            }

            // Hakkında butonuna tıklama işlevi
            aboutButton.setOnClickListener {
                showAboutInfo()
            }
        } catch (e: Exception) {
            Log.e(TAG, "onCreate hatası: ${e.message}", e)
            Toast.makeText(this, "Uygulama başlatılırken hata oluştu: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }

    private fun showAboutInfo() {
        try {
            // Hakkında bilgisini göster
            val aboutView = findViewById<View>(R.id.aboutLayout)
            val closeButton = findViewById<Button>(R.id.closeButton)

            aboutView.visibility = View.VISIBLE
            val fadeIn = AnimationUtils.loadAnimation(this, R.anim.fade_in)
            aboutView.startAnimation(fadeIn)

            closeButton.setOnClickListener {
                aboutView.visibility = View.GONE
            }
        } catch (e: Exception) {
            Log.e(TAG, "Hakkında bilgisi gösterilirken hata: ${e.message}", e)
            Toast.makeText(this, "Hakkında bilgisi gösterilirken hata oluştu: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }
} 